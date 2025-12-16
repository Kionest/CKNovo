using System;
using System.Collections.Generic;
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
    /// Логика взаимодействия для EditTenantWindow.xaml
    /// </summary>
    public partial class EditTenantWindow : Window
    {
        private int _tenantId;

        public EditTenantWindow(int tenantId)
        {
            InitializeComponent();
            _tenantId = tenantId;
            LoadTenant();
        }

        private void LoadTenant()
        {
            using (var db = new NovotekEntities())
            {
                var t = db.Tenants.Find(_tenantId);

                FullNameBox.Text = t.FullName;
                PhoneBox.Text = t.PhoneNumber;
                EmailBox.Text = t.Email;
                AddressBox.Text = t.LegalAddress;
                InnBox.Text = t.INN;
                DirectorBox.Text = t.DirectorName;
                NotesBox.Text = t.Notes;
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(FullNameBox.Text))
            {
                MessageBox.Show("ФИО не может быть пустым");
                return;
            }

            using (var db = new NovotekEntities())
            {
                var t = db.Tenants.Find(_tenantId);

                t.FullName = FullNameBox.Text.Trim();
                t.PhoneNumber = PhoneBox.Text.Trim();
                t.Email = EmailBox.Text.Trim();
                t.LegalAddress = AddressBox.Text.Trim();
                t.INN = InnBox.Text.Trim();
                t.DirectorName = DirectorBox.Text.Trim();
                t.Notes = NotesBox.Text.Trim();

                db.SaveChanges();
            }

            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
