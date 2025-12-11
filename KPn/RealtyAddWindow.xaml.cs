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
    /// Логика взаимодействия для RealtyAddWindow.xaml
    /// </summary>
    public partial class RealtyAddWindow : Window
    {
        public RealtyAddWindow()
        {
            InitializeComponent();
            LoadData();
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

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var db = new NovotekEntities())
                {
                    if (string.IsNullOrWhiteSpace(AddressBox.Text))
                    {
                        MessageBox.Show("Введите адрес.");
                        return;
                    }

                    if (!decimal.TryParse(SquareBox.Text, out decimal square) || square <= 0)
                    {
                        MessageBox.Show("Некорректная площадь.");
                        return;
                    }

                    if (OwnerBox.SelectedValue == null)
                    {
                        MessageBox.Show("Выберите собственника.");
                        return;
                    }

                    if (RealtyTypeBox.SelectedValue == null)
                    {
                        MessageBox.Show("Выберите тип недвижимости.");
                        return;
                    }

                    int? floor = null;
                    if (!string.IsNullOrWhiteSpace(FloorBox.Text))
                    {
                        if (!int.TryParse(FloorBox.Text, out int f))
                        {
                            MessageBox.Show("Этаж должен быть числом.");
                            return;
                        }
                        floor = f;
                    }

                    var newRealty = new Realty
                    {
                        Address = AddressBox.Text.Trim(),
                        SquareMeters = square,
                        Floor = floor,
                        Description = DescriptionBox.Text.Trim(),
                        OwnerID = (int)OwnerBox.SelectedValue,
                        RealtyTypeID = (int)RealtyTypeBox.SelectedValue,
                        IsOwned = OwnedCheckBox.IsChecked ?? false,
                        IsActive = ActiveCheckBox.IsChecked ?? false
                    };

                    db.Realty.Add(newRealty);
                    db.SaveChanges();

                    MessageBox.Show("Объект недвижимости добавлен.");
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

