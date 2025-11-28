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
using System.Windows.Media.Effects;
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

            InitializeGlowAnimations();
        }

        #region Функционирование входа
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

        private void TopPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        #endregion


        #region Градиент
        private void InitializeGlowAnimations()
        {
            Loaded += (s, e) => StartGradientGlowAnimation();
        }

        private void StartGradientGlowAnimation()
        {
            var colorAnimation = new System.Windows.Media.Animation.ColorAnimation
            {
                From = Color.FromRgb(0, 120, 212),
                To = Color.FromRgb(0, 200, 255), 
                Duration = TimeSpan.FromSeconds(3),
                AutoReverse = true,
                RepeatBehavior = System.Windows.Media.Animation.RepeatBehavior.Forever
            };

            var opacityAnimation = new System.Windows.Media.Animation.DoubleAnimation
            {
                From = 0.4,
                To = 0.8,
                Duration = TimeSpan.FromSeconds(2),
                AutoReverse = true,
                RepeatBehavior = System.Windows.Media.Animation.RepeatBehavior.Forever
            };

            GlowEffect.BeginAnimation(DropShadowEffect.ColorProperty, colorAnimation);
            GlowEffect.BeginAnimation(DropShadowEffect.OpacityProperty, opacityAnimation);
        }

        #endregion

    }
}
