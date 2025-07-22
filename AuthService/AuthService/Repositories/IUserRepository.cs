
using AuthService.API.Models;

namespace AuthService.API.Repositories
{
    public interface IUserRepository
    {
        User? GetByUserName(string userName);
        void Add(User user);
        bool ValidateCredentials(string userName, string password);
    }
}
