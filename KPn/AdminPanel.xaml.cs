using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
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
    /// Логика взаимодействия для AdminPanel.xaml
    /// </summary>
    public partial class AdminPanel : Window
    {
        private NovotekEntities _context;
        public AdminPanel()
        {
            InitializeComponent();
            _context = new NovotekEntities();
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                _context.Users.Include(u => u.Role).Load();
                dgUsers.ItemsSource = _context.Users.Local;

                cmbRoles.ItemsSource = _context.Role.ToList();
                if (cmbRoles.Items.Count > 0)
                    cmbRoles.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                ShowMessage($"Ошибка загрузки данных: {ex.Message}", isError: true);
            }
        }

        private void BtnAddUser_Click(object sender, RoutedEventArgs e)
        {
            var login = txtNewLogin.Text.Trim();
            var password = txtNewPassword.Password;
            var selectedRole = cmbRoles.SelectedItem as Role;

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                ShowMessage("Введите логин и пароль", isError: true);
                return;
            }

            if (selectedRole == null)
            {
                ShowMessage("Выберите роль", isError: true);
                return;
            }

            try
            {
                if (_context.Users.Any(u => u.Login == login))
                {
                    ShowMessage("Пользователь с таким логином уже существует", isError: true);
                    return;
                }

                var passwordHash = PasswordHasher.HashPassword(password);

                var newUser = new Users
                {
                    Login = login,
                    PasswordHash = passwordHash,
                    RoleID = selectedRole.RoleID
                };

                _context.Users.Add(newUser);
                _context.SaveChanges();

                txtNewLogin.Clear();
                txtNewPassword.Clear();

                ShowMessage($"Пользователь '{login}' успешно добавлен!", isError: false);
            }
            catch (Exception ex)
            {
                ShowMessage($"Ошибка при добавлении пользователя: {ex.Message}", isError: true);
            }
        }

        private void BtnDeleteUser_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var user = button?.DataContext as Users;

            if (user == null) return;
            if (user.Login == "admin")
            {
                ShowMessage("Нельзя удалить администратора по умолчанию", isError: true);
                return;
            }

            var result = MessageBox.Show(
                $"Вы уверены, что хотите удалить пользователя '{user.Login}'?", "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _context.Users.Remove(user);
                    _context.SaveChanges();
                    ShowMessage($"Пользователь '{user.Login}' удален", isError: false);
                }
                catch (Exception ex)
                {
                    ShowMessage($"Ошибка при удалении пользователя: {ex.Message}", isError: true);
                }
            }
        }

        private void ShowMessage(string message, bool isError)
        {
            txtMessage.Text = message;
            txtMessage.Foreground = isError ? System.Windows.Media.Brushes.Red : System.Windows.Media.Brushes.Green;
        }

        protected override void OnClosed(EventArgs e)
        {
            _context?.Dispose();
            base.OnClosed(e);
        }
    }
}
