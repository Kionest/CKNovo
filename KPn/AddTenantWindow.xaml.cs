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
    /// Логика взаимодействия для AddTenantWindow.xaml
    /// </summary>
    public partial class AddTenantWindow : Window
    {
        public AddTenantWindow()
        {
            InitializeComponent();
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(FullNameBox.Text))
            {
                MessageBox.Show("Введите ФИО или название клиента");
                return;
            }

            if (!string.IsNullOrWhiteSpace(EmailBox.Text) &&
                !EmailBox.Text.Contains("@"))
            {
                MessageBox.Show("Некорректный Email");
                return;
            }

            using (var db = new NovotekEntities())
            {
                var tenant = new Tenants
                {
                    FullName = FullNameBox.Text.Trim(),
                    PhoneNumber = PhoneBox.Text.Trim(),
                    Email = EmailBox.Text.Trim(),
                    LegalAddress = AddressBox.Text.Trim(),
                    INN = InnBox.Text.Trim(),
                    DirectorName = DirectorBox.Text.Trim(),
                    Notes = NotesBox.Text.Trim()
                };

                db.Tenants.Add(tenant);
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
