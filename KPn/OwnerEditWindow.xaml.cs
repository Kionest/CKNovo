using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace KPn
{
    /// <summary>
    /// Логика взаимодействия для OwnerEditWindow.xaml
    /// </summary>
    public partial class OwnerEditWindow : Window
    {
        private int ownerId;

        public OwnerEditWindow(int id)
        {
            InitializeComponent();
            ownerId = id;
            LoadOwner();
        }

        private void LoadOwner()
        {
            using (var db = new NovotekEntities())
            {
                var owner = db.Owners.FirstOrDefault(o => o.OwnerID == ownerId);
                if (owner == null)
                {
                    MessageBox.Show("Ошибка загрузки данных.");
                    Close();
                    return;
                }

                FullNameBox.Text = owner.FullName;
                PhoneBox.Text = owner.PhoneNumber;
                EmailBox.Text = owner.Email;
                AddressBox.Text = owner.Address;
                INNBox.Text = owner.INN;
                BankBox.Text = owner.BankDetails;
                NotesBox.Text = owner.Notes;
            }
        }

        private bool ValidateFields()
        {
            if (string.IsNullOrWhiteSpace(FullNameBox.Text))
            {
                MessageBox.Show("Введите ФИО.");
                return false;
            }

            if (!Regex.IsMatch(PhoneBox.Text, @"^\d{10,12}$"))
            {
                MessageBox.Show("Телефон должен содержать 10–12 цифр.");
                return false;
            }

            if (!Regex.IsMatch(EmailBox.Text, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                MessageBox.Show("Введите корректный Email.");
                return false;
            }

            if (!Regex.IsMatch(INNBox.Text, @"^\d{10}|\d{12}$"))
            {
                MessageBox.Show("ИНН должен содержать 10 или 12 цифр.");
                return false;
            }

            return true;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateFields())
                return;

            using (var db = new NovotekEntities())
            {
                var owner = db.Owners.FirstOrDefault(o => o.OwnerID == ownerId);
                if (owner == null)
                {
                    MessageBox.Show("Ошибка сохранения.");
                    return;
                }

                if (db.Owners.Any(o => o.Email == EmailBox.Text && o.OwnerID != ownerId))
                {
                    MessageBox.Show("Этот Email уже используется другим собственником.");
                    return;
                }

                if (db.Owners.Any(o => o.INN == INNBox.Text && o.OwnerID != ownerId))
                {
                    MessageBox.Show("Этот ИНН уже существует.");
                    return;
                }

                owner.FullName = FullNameBox.Text;
                owner.PhoneNumber = PhoneBox.Text;
                owner.Email = EmailBox.Text;
                owner.Address = AddressBox.Text;
                owner.INN = INNBox.Text;
                owner.BankDetails = BankBox.Text;
                owner.Notes = NotesBox.Text;

                db.SaveChanges();
            }

            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}

