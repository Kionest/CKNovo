using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Text;
using System.Threading.Tasks;

namespace KPn
{
    public class AuthService
    {
        private NovotekEntities _context;

        public AuthService()
        {
            _context = new NovotekEntities();
        }

        public Users Authenticate(string login, string password)
        {
            var user = _context.Users
                .Include(u => u.Role)
                .FirstOrDefault(u => u.Login == login);

            if (user != null && PasswordHasher.VerifyPassword(password, user.PasswordHash))
            {
                return user;
            }

            return null;
        }

        public bool RegisterUser(string login, string password, int roleId = 2)
        {
            try
            {
                if (_context.Users.Any(u => u.Login == login))
                {
                    return false;
                }

                var passwordHash = PasswordHasher.HashPassword(password);

                var newUser = new Users
                {
                    Login = login,
                    PasswordHash = passwordHash,
                    RoleID = roleId
                };

                _context.Users.Add(newUser);
                _context.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка регистрации: {ex.Message}");
                return false;
            }
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}
