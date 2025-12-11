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
    /// Логика взаимодействия для RealtyEditWindow.xaml
    /// </summary>
    public partial class RealtyEditWindow : Window
    {
        private int _id;

        public RealtyEditWindow(int realtyId)
        {
            InitializeComponent();
            _id = realtyId;

            LoadData();
            LoadRealty();
        }

        private void LoadData()
        {
            using (var db = new NovotekEntities())
            {
                OwnerBox.ItemsSource = db.Owners
                    .Select(o => new { o.OwnerID, o.FullName })
                    .ToList();
                OwnerBox.DisplayMemberPath = "FullName";
                OwnerBox.SelectedValuePath = "OwnerID";

                RealtyTypeBox.ItemsSource = db.RealtyTypes
                    .Select(t => new { t.RealtyTypeID, t.TypeName })
                    .ToList();
                RealtyTypeBox.DisplayMemberPath = "TypeName";
                RealtyTypeBox.SelectedValuePath = "RealtyTypeID";
            }
        }

        private void LoadRealty()
        {
            using (var db = new NovotekEntities())
            {
                var obj = db.Realty.Find(_id);

                AddressBox.Text = obj.Address;
                SquareBox.Text = obj.SquareMeters.ToString();
                FloorBox.Text = obj.Floor?.ToString();
                DescriptionBox.Text = obj.Description;

                OwnerBox.SelectedValue = obj.OwnerID;
                RealtyTypeBox.SelectedValue = obj.RealtyTypeID;

                OwnedCheckBox.IsChecked = obj.IsOwned;
                ActiveCheckBox.IsChecked = obj.IsActive;
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var db = new NovotekEntities())
                {
                    var obj = db.Realty.Find(_id);

                    obj.Address = AddressBox.Text.Trim();

                    if (!decimal.TryParse(SquareBox.Text, out decimal square) || square <= 0)
                    {
                        MessageBox.Show("Некорректная площадь.");
                        return;
                    }
                    obj.SquareMeters = square;

                    if (!string.IsNullOrWhiteSpace(FloorBox.Text))
                    {
                        if (!int.TryParse(FloorBox.Text, out int f))
                        {
                            MessageBox.Show("Этаж должен быть числом.");
                            return;
                        }
                        obj.Floor = f;
                    }
                    else
                    {
                        obj.Floor = null;
                    }

                    obj.Description = DescriptionBox.Text.Trim();

                    obj.OwnerID = (int)OwnerBox.SelectedValue;
                    obj.RealtyTypeID = (int)RealtyTypeBox.SelectedValue;

                    obj.IsOwned = OwnedCheckBox.IsChecked ?? false;
                    obj.IsActive = ActiveCheckBox.IsChecked ?? false;

                    db.SaveChanges();

                    MessageBox.Show("Данные сохранены.");
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
