using AuthService.API.Models;

namespace AuthService.API.Services
{
    public interface IAuthService
    {
        LoginResponse Login(string userName, string password);
        void Logout(string token);
        bool IsAuthenticated(string token);
        User? GetUserFromToken(string token);
    }
}
