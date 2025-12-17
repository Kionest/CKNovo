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
            LoadUserStatistics();
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

        #region Работа с пользователями
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

            LoadUserStatistics();
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
        #endregion

        #region Карточки
        private void LoadUserStatistics()
        {
            using (var db = new NovotekEntities())
            {
                var users = db.Users.ToList();
                var roles = db.Role.ToList();

                TotalUsersText.Text = users.Count.ToString();

                ActiveUsersText.Text = $"Активных: {users.Count(u => u.IsActive)}";

                RolesStatsPanel.Children.Clear();

                foreach (var role in roles)
                {
                    int count = users.Count(u => u.RoleID == role.RoleID);

                    var roleText = new TextBlock
                    {
                        Text = $"{role.Name}: {count}",
                        FontSize = 13,
                        Margin = new Thickness(0, 2, 0, 2),
                        Foreground = GetRoleColor(role.Name)
                    };

                    RolesStatsPanel.Children.Add(roleText);
                }

                DateTime today = DateTime.Today;
                UsersTodayText.Text = $"Сегодня: {users.Count(u => u.CreatedAt >= today)}";
                UsersWeekText.Text = $"За неделю: {users.Count(u => u.CreatedAt >= today.AddDays(-7))}";
                UsersMonthText.Text = $"За месяц: {users.Count(u => u.CreatedAt >= today.AddMonths(-1))}";
            }
        }

        private Brush GetRoleColor(string roleName)
        {
            switch (roleName)
            {
                case "Администратор":
                    return new SolidColorBrush(Colors.DarkRed);

                case "Менеджер":
                    return new SolidColorBrush(Colors.DarkBlue);

                case "Пользователь":
                    return new SolidColorBrush(Colors.DarkGreen);

                default:
                    return new SolidColorBrush(Colors.Black);
            }
        }
        #endregion


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

        private void AboutDeveloper_Click(object sender, RoutedEventArgs e)
        {
            DeveloperInfoWindow developerWindow = new DeveloperInfoWindow();
            developerWindow.Show();
        }
    }
}
