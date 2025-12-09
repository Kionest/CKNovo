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
    /// Логика взаимодействия для EditContractWindow.xaml
    /// </summary>
    public partial class EditContractWindow : Window
    {
        private int _contractId;

        public EditContractWindow(int contractId)
        {
            InitializeComponent();
            _contractId = contractId;

            LoadLists();
            LoadContract();
        }

        private void LoadLists()
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

        private void LoadContract()
        {
            using (var db = new NovotekEntities())
            {
                var contract = db.Contracts.FirstOrDefault(c => c.ContractID == _contractId);

                if (contract == null)
                {
                    MessageBox.Show("Ошибка загрузки договора.");
                    Close();
                    return;
                }

                NumBox.Text = contract.ContractNumber;
                RealtyBox.SelectedValue = contract.RealtyID;
                TenantBox.SelectedValue = contract.TenantID;
                StatusBox.SelectedValue = contract.ContractStatusID;

                StartDatePicker.SelectedDate = contract.StartDate;
                EndDatePicker.SelectedDate = contract.EndDate;

                RentBox.Text = contract.MonthlyRent.ToString();
                DepositBox.Text = contract.Deposit.ToString();
                DueDayBox.Text = contract.PaymentDueDay.ToString();
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new NovotekEntities())
            {
                var contract = db.Contracts.FirstOrDefault(c => c.ContractID == _contractId);

                if (contract == null)
                {
                    MessageBox.Show("Не удалось найти договор.");
                    return;
                }

                contract.ContractNumber = NumBox.Text.Trim();
                contract.RealtyID = (int)RealtyBox.SelectedValue;
                contract.TenantID = (int)TenantBox.SelectedValue;
                contract.ContractStatusID = (int)StatusBox.SelectedValue;

                contract.StartDate = StartDatePicker.SelectedDate.Value;
                contract.EndDate = EndDatePicker.SelectedDate.Value;

                contract.MonthlyRent = decimal.Parse(RentBox.Text);
                contract.Deposit = decimal.Parse(DepositBox.Text);

                contract.PaymentDueDay = byte.Parse(DueDayBox.Text);

                db.SaveChanges();
            }

            MessageBox.Show("Договор обновлён!");
            Close();
        }
    }
}
