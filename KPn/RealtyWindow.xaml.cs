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
    /// Логика взаимодействия для RealtyWindow.xaml
    /// </summary>
    public partial class RealtyWindow : Window
    {
        private NovotekEntities db = new NovotekEntities();
        private Realty editing;

        public RealtyWindow(Realty realty = null)
        {
            InitializeComponent();

            OwnerBox.ItemsSource = db.Owners.ToList();
            TypeBox.ItemsSource = db.RealtyTypes.ToList();

            if (realty == null)
            {
                editing = new Realty();
            }
            else
            {
                editing = realty;
                FillFields();
            }
        }

        private void FillFields()
        {
            AddressBox.Text = editing.Address;
            SquareBox.Text = editing.SquareMeters.ToString();
            FloorBox.Text = editing.Floor.ToString();
            OwnerBox.SelectedValue = editing.OwnerID;
            TypeBox.SelectedValue = editing.RealtyTypeID;
            IsOwnedBox.IsChecked = editing.IsOwned;
            IsActiveBox.IsChecked = editing.IsActive;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                editing.Address = AddressBox.Text;
                editing.SquareMeters = int.Parse(SquareBox.Text);
                editing.Floor = int.Parse(FloorBox.Text);
                editing.OwnerID = (int)OwnerBox.SelectedValue;
                editing.RealtyTypeID = (int)TypeBox.SelectedValue;
                editing.IsOwned = IsOwnedBox.IsChecked ?? false;
                editing.IsActive = IsActiveBox.IsChecked ?? false;

                db.Entry(editing).State =
                    editing.RealtyID == 0 ?
                    EntityState.Added :
                    EntityState.Modified;

                db.SaveChanges();

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
