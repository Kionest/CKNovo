using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace KPn
{
    /// <summary>
    /// Логика взаимодействия для ManagerWindow.xaml
    /// </summary>
    public partial class ManagerWindow : Window
    {
        private NovotekEntities db = new NovotekEntities();

        private Button _currentButton;
        private List<Tenants> allTenants;
        private List<Realty> allRealty;

        public ManagerWindow()
        {
            InitializeComponent();
            LoadRealty();
            LoadTenants();

            SetActiveButton(BtnObjects);
            ShowPanel(ObjectsPanel);
        }

        #region Недвижимость

        private void LoadRealty()
        {
            RealtyGrid.ItemsSource = db.Realty.ToList();
            allRealty = db.Realty.ToList();
            ApplyRealtyFilter();
            UpdateRealtyStats();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (RealtyGrid.SelectedItem is Realty selected)
            {
                db.Realty.Remove(selected);
                db.SaveChanges();
                LoadRealty();
            }
            else
            {
                MessageBox.Show("Выберите запись для удаления.");
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var win = new RealtyWindow();
            if (win.ShowDialog() == true)
                LoadRealty();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (RealtyGrid.SelectedItem is Realty selected)
            {
                var win = new RealtyWindow(selected);
                if (win.ShowDialog() == true)
                    LoadRealty();
            }
            else
            {
                MessageBox.Show("Выберите запись для редактирования.");
            }
        }

        private void UpdateRealtyStats()
        {
            if (RealtyGrid.ItemsSource == null) return;

            var list = RealtyGrid.ItemsSource.Cast<Realty>().ToList();

            int total = list.Count;
            int active = list.Count(r => r.IsActive);
            int owned = list.Count(r => r.IsOwned);

            double activePercent = total == 0 ? 0 : (active / (double)total * 100);
            double ownedPercent = total == 0 ? 0 : (owned / (double)total * 100);

            TotalRealtyText.Text = total.ToString();
            ActiveRealtyText.Text = $"Активные: {active} ({activePercent:F0}%)";

            OwnedRealtyText.Text = owned.ToString();
            OwnedPercentText.Text = $"Процент собственных: {ownedPercent:F0}%";

            OwnersStatsPanel.Children.Clear();

            var owners = list
                .GroupBy(r => r.Owners?.FullName ?? "Собственник не указан")
                .Select(g => new { Owner = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count);

            foreach (var o in owners)
            {
                OwnersStatsPanel.Children.Add(new TextBlock
                {
                    Text = $"{o.Owner}: {o.Count}",
                    FontSize = 16,
                    Margin = new Thickness(0, 3, 0, 3),
                    Foreground = new SolidColorBrush(Color.FromRgb(50, 50, 50))
                });
            }
        }

        private void RealtyFilters_Changed(object sender, EventArgs e)
        {
            ApplyRealtyFilter();
        }

        private void ApplyRealtyFilter()
        {
            if (allRealty == null) return;

            string search = RealtySearchBox.Text?.ToLower() ?? "";
            string status = (StatusFilter.SelectedItem as ComboBoxItem)?.Content.ToString();

            var filtered = allRealty.Where(r => (string.IsNullOrWhiteSpace(search) || (r.Address != null && r.Address.ToLower().Contains(search)))).ToList();

            switch (status)
            {
                case "Активные":
                    filtered = filtered.Where(r => r.IsActive).ToList();
                    break;

                case "Неактивные":
                    filtered = filtered.Where(r => !r.IsActive).ToList();
                    break;

                case "Собственные":
                    filtered = filtered.Where(r => r.IsOwned).ToList();
                    break;

                case "Несобственные":
                    filtered = filtered.Where(r => !r.IsOwned).ToList();
                    break;
            }

            RealtyGrid.ItemsSource = filtered;
        }

        #endregion


        #region Клиенты
        private void LoadTenants()
        {
            allTenants = db.Tenants.ToList();
            TenantsGrid.ItemsSource = db.Tenants.ToList();
            UpdateTenantStats();
        }

        private void UpdateTenantStats()
        {
            if (allTenants == null || allTenants.Count == 0)
            {
                TotalClientsText.Text = "0";
                CompanyClientsText.Text = "0";
                IndividualClientsText.Text = "0";
                CompanyPercentText.Text = "0% от общего числа";
                IndividualPercentText.Text = "0% от общего числа";
                return;
            }

            int total = allTenants.Count;
            int companies = allTenants.Count(t => t.INN != null && t.INN.Length == 10);
            int ip = allTenants.Count(t => t.INN != null && t.INN.Length == 12);
            ip += allTenants.Count(t => string.IsNullOrWhiteSpace(t.INN));

            TotalClientsText.Text = total.ToString();
            CompanyClientsText.Text = companies.ToString();
            IndividualClientsText.Text = ip.ToString();

            int companyPercent = companies * 100 / total;
            int ipPercent = ip * 100 / total;

            CompanyPercentText.Text = $"{companyPercent}% от общего числа";
            IndividualPercentText.Text = $"{ipPercent}% от общего числа";
        }

        private void AddTenantButton_Click(object sender, RoutedEventArgs e)
        {
            TenantAddWindow win = new TenantAddWindow();

            if (win.ShowDialog() == true)
            {
                using (var db = new NovotekEntities())
                {
                    db.Tenants.Add(win.NewTenant);
                    db.SaveChanges();
                }

                LoadTenants();
            }
        }

        private void DeleteTenantButton_Click(object sender, RoutedEventArgs e)
        {
            var selected = TenantsGrid.SelectedItem as Tenants;

            if (selected == null)
            {
                MessageBox.Show("Выберите клиента.");
                return;
            }

            using (var db = new NovotekEntities())
            {
                var tenant = db.Tenants.Find(selected.TenantID);

                db.Tenants.Remove(tenant);
                db.SaveChanges();
            }

            LoadTenants();
        }

        private void TenantSearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string text = TenantSearchBox.Text.ToLower();

            var filtered = allTenants
                .Where(t =>
                    (t.FullName != null && t.FullName.ToLower().Contains(text)) ||
                    (t.PhoneNumber != null && t.PhoneNumber.ToLower().Contains(text)) ||
                    (t.Email != null && t.Email.ToLower().Contains(text)) ||
                    (t.INN != null && t.INN.ToLower().Contains(text)) ||
                    (t.DirectorName != null && t.DirectorName.ToLower().Contains(text))
                )
                .ToList();

            TenantsGrid.ItemsSource = filtered;
        }
        #endregion


        #region Методы для работы окна
        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                SetActiveButton(button);

                switch (button.Tag.ToString())
                {
                    case "0":
                        ShowPanel(ObjectsPanel);
                        break;
                    case "1":
                        ShowPanel(ClientsPanel);
                        break;
                    case "2":
                        ShowPanel(ContractsPanel);
                        break;
                    case "3":
                        ShowPanel(MapPanel);
                        break;
                }
            }
        }

        private void SetActiveButton(Button activeButton)
        {
            if (_currentButton != null)
            {
                _currentButton.ClearValue(Button.BackgroundProperty);
                _currentButton.ClearValue(Button.ForegroundProperty);
            }

            activeButton.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#173F5F");
            activeButton.Foreground = Brushes.White;
            _currentButton = activeButton;
        }

        private void ShowPanel(StackPanel panelToShow)
        {
            ObjectsPanel.Visibility = Visibility.Collapsed;
            ClientsPanel.Visibility = Visibility.Collapsed;
            ContractsPanel.Visibility = Visibility.Collapsed;
            MapPanel.Visibility = Visibility.Collapsed;
            panelToShow.Visibility = Visibility.Visible;
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Вы действительно хотите выйти?", "Выход", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                this.Close();
            }
        }

        #endregion
    }
}

