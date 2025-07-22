using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using APIGatewayService.Models;

namespace APIGatewayService.Services
{
    public class AuthServiceClient : IAuthenticationService
    {
        private readonly HttpClient _httpClient;

        public AuthServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<UserDetailsDto?> ValidateTokenAsync(string token)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "api/Auth/profile");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<UserDetailsDto>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
    }
}