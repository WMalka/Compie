using System.Threading.Tasks;
using APIGatewayService.Models;

namespace APIGatewayService.Services
{
    public interface IAuthenticationService
    {
        Task<UserDetailsDto?> ValidateTokenAsync(string token);
    }
}