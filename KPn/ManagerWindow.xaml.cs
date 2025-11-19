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
using System.Windows.Threading;

namespace KPn
{
    /// <summary>
    /// Логика взаимодействия для ManagerWindow.xaml
    /// </summary>
    public partial class ManagerWindow : Window
    {
        private NovotekEntities _context;
        private string _currentTableName;

        public ManagerWindow()
        {
            InitializeComponent();
            InitializeManagerPanel();
        }

        private void InitializeManagerPanel()
        {
            _context = new NovotekEntities();
            LoadInitialData();
            UpdateStatus("Панель менеджера загружена");
        }

        private void LoadInitialData()
        {
            try
            {
                cmbTables.SelectedIndex = 0;
                LoadTableData("Realty");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка");
            }
        }


        private void CmbTables_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbTables.SelectedItem is ComboBoxItem item)
            {
                string tableName = item.Tag.ToString();
                LoadTableData(tableName);
            }
        }

        private void LoadTableData(string tableName)
        {
            try
            {
                _currentTableName = tableName;
                txtCurrentTable.Text = GetTableDisplayName(tableName);

                switch (tableName)
                {
                    case "Realty":
                        _context.Realty
                            .Include(r => r.RealtyTypes)
                            .Include(r => r.Owners)
                            .Load();
                        dgData.ItemsSource = _context.Realty.Local;
                        break;

                    case "Contracts":
                        _context.Contracts
                            .Include(c => c.Realty)
                            .Include(c => c.Tenants)
                            .Include(c => c.ContractStatuses)
                            .Load();
                        dgData.ItemsSource = _context.Contracts.Local;
                        break;

                    case "Tenants":
                        _context.Tenants.Load();
                        dgData.ItemsSource = _context.Tenants.Local;
                        break;

                    case "Payments":
                        _context.Payments
                            .Include(p => p.Contracts)
                            .Load();
                        dgData.ItemsSource = _context.Payments.Local;
                        break;

                    case "Owners":
                        _context.Owners.Load();
                        dgData.ItemsSource = _context.Owners.Local;
                        break;

                    case "RealtyTypes":
                        _context.RealtyTypes.Load();
                        dgData.ItemsSource = _context.RealtyTypes.Local;
                        break;

                    case "ContractStatuses":
                        _context.ContractStatuses.Load();
                        dgData.ItemsSource = _context.ContractStatuses.Local;
                        break;
                }

                UpdateRecordsCount();
                UpdateStatus($"Загружена таблица: {GetTableDisplayName(tableName)}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки таблицы: {ex.Message}", "Ошибка");
            }
        }

        private string GetTableDisplayName(string tableName)
        {
            switch (tableName)
            {
                case "Realty": return "Объекты недвижимости";
                case "Contracts": return "Договоры аренды";
                case "Tenants": return "Арендаторы";
                case "Payments": return "Платежи";
                case "Owners": return "Владельцы";
                case "RealtyTypes": return "Типы недвижимости";
                case "ContractStatuses": return "Статусы договоров";
                default: return tableName;
            }
        }


        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (dgData.SelectedItem == null)
            {
                MessageBox.Show("Выберите запись для редактирования");
                return;
            }

            dgData.IsReadOnly = false;
            UpdateStatus("Режим редактирования включен");
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                switch (_currentTableName)
                {
                    case "Realty":
                        _context.Realty.Add(new Realty { IsActive = true });
                        break;
                    case "Contracts":
                        _context.Contracts.Add(new Contracts { ContractStatusID = 1 });
                        break;
                    case "Tenants":
                        _context.Tenants.Add(new Tenants());
                        break;
                    case "Payments":
                        _context.Payments.Add(new Payments { PaymentDate = DateTime.Now });
                        break;
                    case "Owners":
                        _context.Owners.Add(new Owners());
                        break;
                    case "RealtyTypes":
                        _context.RealtyTypes.Add(new RealtyTypes());
                        break;
                    case "ContractStatuses":
                        _context.ContractStatuses.Add(new ContractStatuses());
                        break;
                }

                _context.SaveChanges();
                LoadTableData(_currentTableName);
                UpdateStatus("Новая запись добавлена");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка добавления: {ex.Message}", "Ошибка");
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dgData.SelectedItem == null)
            {
                MessageBox.Show("Выберите запись для удаления");
                return;
            }

            var result = MessageBox.Show("Удалить выбранную запись?", "Подтверждение",
                                       MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var item = dgData.SelectedItem;
                    _context.Entry(item).State = EntityState.Deleted;
                    _context.SaveChanges();

                    LoadTableData(_currentTableName);
                    UpdateStatus("Запись удалена");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка удаления: {ex.Message}", "Ошибка");
                }
            }
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(_currentTableName))
            {
                LoadTableData(_currentTableName);
                UpdateStatus("Данные обновлены");
            }
        }

        private void BtnReports_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Раздел отчетов в разработке");
        }
      

        private void DgData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }

        private void DgData_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            try
            {
                _context.SaveChanges();
                UpdateStatus("Изменения сохранены");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка");
            }
        }



        private void UpdateStatus(string message)
        {
            txtStatus.Text = message;
        }

        private void UpdateRecordsCount()
        {
            int count = dgData.Items.Count;
            txtRecordsCount.Text = $"Записей: {count}";
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Выйти из системы?", "Выход", MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes)
            {
                var loginWindow = new LoginWindow();
                loginWindow.Show();
                this.Close();
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            _context?.Dispose();
            base.OnClosed(e);
        }

    }
}
