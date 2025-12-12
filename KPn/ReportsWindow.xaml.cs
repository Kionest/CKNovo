using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
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
    /// Логика взаимодействия для ReportsWindow.xaml
    /// </summary>
    public partial class ReportsWindow : Window
    {
        private readonly NovotekEntities db = new NovotekEntities();
        private readonly ContractGeneratorFacade facade;


        public ReportsWindow()
        {
            InitializeComponent();
            facade = new ContractGeneratorFacade(db);
            LoadContracts();
        }


        private void LoadContracts()
        {
            try
            {
                var list = db.Contracts
                    .OrderByDescending(c => c.StartDate)
                    .Take(200)
                    .ToList();

                foreach (var contract in list)
                {
                    if (contract.TenantID > 0)
                    {
                        contract.Tenants = db.Tenants.FirstOrDefault(t => t.TenantID == contract.TenantID);
                    }

                    if (contract.RealtyID > 0)
                    {
                        contract.Realty = db.Realty.FirstOrDefault(r => r.RealtyID == contract.RealtyID);
                    }

                    if (contract.ContractStatusID > 0)
                    {
                        contract.ContractStatuses = db.ContractStatuses.FirstOrDefault(s => s.ContractStatusID == contract.ContractStatusID);
                    }
                }

                ContractsGrid.ItemsSource = list;
                Console.WriteLine($"Загружено {list.Count} контрактов");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при загрузке: {ex.Message}");

                var simpleList = db.Contracts
                    .OrderByDescending(c => c.StartDate)
                    .Take(200)
                    .ToList();

                ContractsGrid.ItemsSource = simpleList;

            }
        }


        private void RefreshBtn_Click(object sender, RoutedEventArgs e)
        {
            LoadContracts();
        }


        private void GenerateBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ContractsGrid.SelectedItem is Contracts contract)
            {
                string templateType = (TemplateCombo.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Individual";
                try
                {
                    ContractModel model = new ContractModel(contract);

                    string outPath = facade.GenerateContract(model, templateType);
                    MessageBox.Show($"Готово. Файл: {outPath}", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                    var folder = System.IO.Path.GetDirectoryName(outPath);
                    if (Directory.Exists(folder))
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
                        {
                            FileName = folder,
                            UseShellExecute = true
                        });
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Выберите контракт в списке.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}

