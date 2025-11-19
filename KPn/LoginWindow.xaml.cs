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
    /// Логика взаимодействия для LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private AuthService _authService;

        public Users CurrentUser { get; private set; }

        public LoginWindow()
        {
            InitializeComponent();
            _authService = new AuthService();
            txtLogin.Focus();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            var login = txtLogin.Text.Trim();
            var password = txtPassword.Password;

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                ShowError("Введите логин и пароль");
                return;
            }

            try
            {
                var user = _authService.Authenticate(login, password);

                if (user != null)
                {
                    if (user.RoleID == 1)
                    {
                        AdminPanel windowAdmin = new AdminPanel();
                        windowAdmin.Show();
                        this.Close();
                    }
                    else if (user.RoleID == 2)
                    {
                        ManagerWindow managerWindow = new ManagerWindow();
                        managerWindow.Show();
                        this.Close();
                    }
                    else if (user.RoleID == 3)
                    {
                        AccountantWindow accountantWindow = new AccountantWindow();
                        accountantWindow.Show();
                        this.Close();
                    }
                    else
                    {
                        MainWindow mainWindow = new MainWindow();
                        mainWindow.Show();
                        this.Close();
                    }
                }
                else
                {
                    ShowError("Неверный логин или пароль");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Ошибка авторизации: {ex.Message}");
            }
        }

        private void ShowError(string message)
        {
            txtError.Text = message;
            txtError.Visibility = Visibility.Visible;
        }

        protected override void OnClosed(EventArgs e)
        {
            _authService?.Dispose();
            base.OnClosed(e);
        }
    }
}
