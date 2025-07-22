using APIGatewayService.Models;
using APIGatewayService.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace APIGatewayService.Controllers
{
    [ApiController]
    [Route("api/gateway")]
    public class APIGatewayController : ControllerBase
    {
        private readonly IAuthenticationService _authServiceClient;
        private readonly ILogger<APIGatewayController> _logger;

        public APIGatewayController(IAuthenticationService authServiceClient, ILogger<APIGatewayController> logger)
        {
            _authServiceClient = authServiceClient;
            _logger = logger;
        }

        [HttpGet("user/me")]
        public async Task<IActionResult> GetUserDetails()
        {
            var token = ExtractBearerToken(Request.Headers["Authorization"]);
            if (string.IsNullOrWhiteSpace(token))
            {
                _logger.LogWarning("Unauthorized access attempt: Missing or invalid Authorization header.");
                return Unauthorized();
            }

            var user = await _authServiceClient.ValidateTokenAsync(token);

            if (user == null)
            {
                _logger.LogWarning("Unauthorized access attempt: Invalid or expired token.");
                return Unauthorized();
            }

            _logger.LogInformation("User '{UserName}' successfully authenticated.", user.UserName);
            return Ok(user);
        }

        private static string? ExtractBearerToken(string authHeader)
        {
            if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer "))
                return null;
            return authHeader.Substring("Bearer ".Length).Trim();
        }
    }
}