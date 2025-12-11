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
    /// Логика взаимодействия для AddOwnerWindow.xaml
    /// </summary>
    public partial class AddOwnerWindow : Window
    {
        public AddOwnerWindow()
        {
            InitializeComponent();
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
            if (!ValidateFields()) return;

            using (var db = new NovotekEntities())
            {
                if (db.Owners.Any(o => o.Email == EmailBox.Text))
                {
                    MessageBox.Show("Собственник с таким Email уже существует.");
                    return;
                }

                if (db.Owners.Any(o => o.INN == INNBox.Text))
                {
                    MessageBox.Show("Собственник с таким ИНН уже существует.");
                    return;
                }

                Owners owner = new Owners
                {
                    FullName = FullNameBox.Text,
                    PhoneNumber = PhoneBox.Text,
                    Email = EmailBox.Text,
                    Address = AddressBox.Text,
                    INN = INNBox.Text,
                    BankDetails = BankBox.Text,
                    Notes = NotesBox.Text
                };

                db.Owners.Add(owner);
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

