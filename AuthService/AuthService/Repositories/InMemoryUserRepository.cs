using AuthService.API.Models;
using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;

namespace AuthService.API.Repositories
{
    public class InMemoryUserRepository : IUserRepository
    {
        private readonly ConcurrentDictionary<string, User> _users = new();

        public InMemoryUserRepository()
        {
            // Seed with a test user: userName "admin", password "password"
            Add(new User
            {
                UserName = "admin",
                PasswordHash = HashPassword("password"),
                Role = "Admin"
            });
        }

        public User? GetByUserName(string userName)
            => _users.TryGetValue(userName, out var user) ? user : null;

        public void Add(User user)
            => _users.TryAdd(user.UserName, user);

        public bool ValidateCredentials(string userName, string password)
        {
            var user = GetByUserName(userName);
            if (user == null) return false;
            return user.PasswordHash == HashPassword(password);
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
    }
}
