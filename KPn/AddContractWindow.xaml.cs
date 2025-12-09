using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
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
    /// Логика взаимодействия для AddContractWindow.xaml
    /// </summary>
    public partial class AddContractWindow : Window
    {
        public AddContractWindow()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            using (var db = new NovotekEntities())
            {
                RealtyBox.ItemsSource = db.Realty
                    .Select(r => new { r.RealtyID, r.Address })
                    .ToList();
                RealtyBox.DisplayMemberPath = "Address";
                RealtyBox.SelectedValuePath = "RealtyID";

                TenantBox.ItemsSource = db.Tenants
                    .Select(t => new { t.TenantID, t.FullName })
                    .ToList();
                TenantBox.DisplayMemberPath = "FullName";
                TenantBox.SelectedValuePath = "TenantID";

                StatusBox.ItemsSource = db.ContractStatuses
                    .Select(s => new { s.ContractStatusID, s.StatusName })
                    .ToList();
                StatusBox.DisplayMemberPath = "StatusName";
                StatusBox.SelectedValuePath = "ContractStatusID";
            }
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var db = new NovotekEntities())
                {
                    if (string.IsNullOrWhiteSpace(NumBox.Text))
                    {
                        MessageBox.Show("Введите номер договора.");
                        return;
                    }

                    if (RealtyBox.SelectedValue == null ||
                        TenantBox.SelectedValue == null ||
                        StatusBox.SelectedValue == null)
                    {
                        MessageBox.Show("Заполните все выпадающие списки.");
                        return;
                    }

                    if (StartDatePicker.SelectedDate == null ||
                        EndDatePicker.SelectedDate == null)
                    {
                        MessageBox.Show("Укажите даты.");
                        return;
                    }

                    if (!decimal.TryParse(RentBox.Text, out decimal rent) ||
                        rent < 0)
                    {
                        MessageBox.Show("Некорректная сумма аренды.");
                        return;
                    }

                    if (!decimal.TryParse(DepositBox.Text, out decimal deposit) ||
                        deposit < 0)
                    {
                        MessageBox.Show("Некорректная сумма депозита.");
                        return;
                    }

                    var newContract = new Contracts
                    {
                        ContractNumber = NumBox.Text.Trim(),
                        RealtyID = (int)RealtyBox.SelectedValue,
                        TenantID = (int)TenantBox.SelectedValue,
                        ContractStatusID = (int)StatusBox.SelectedValue,
                        StartDate = StartDatePicker.SelectedDate.Value,
                        EndDate = EndDatePicker.SelectedDate.Value,
                        MonthlyRent = rent,
                        Deposit = deposit
                    };

                    db.Contracts.Add(newContract);
                    db.SaveChanges();

                    MessageBox.Show("Договор создан.");
                    DialogResult = true;
                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
