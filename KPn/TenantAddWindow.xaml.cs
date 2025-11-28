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
    /// Логика взаимодействия для TenantAddWindow.xaml
    /// </summary>
    public partial class TenantAddWindow : Window
    {
        public Tenants NewTenant { get; set; }

        public TenantAddWindow()
        {
            InitializeComponent();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            NewTenant = new Tenants
            {
                FullName = FullNameBox.Text,
                PhoneNumber = PhoneBox.Text,
                Email = EmailBox.Text,
                LegalAddress = AddressBox.Text,
                INN = InnBox.Text,
                DirectorName = DirectorBox.Text,
                Notes = NotesBox.Text
            };

            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
