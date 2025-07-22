using AuthService.API.Models;
using AuthService.API.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Concurrent;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthService.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<AuthService> _logger;
        private readonly string _jwtSecret;
        private readonly int _jwtLifespanMinutes;
        private static readonly ConcurrentDictionary<string, DateTime> _revokedTokens = new();


        public AuthService(IUserRepository userRepository, ILogger<AuthService> logger, IConfiguration config)
        {
            _userRepository = userRepository;
            _logger = logger;
            _jwtSecret = config["Jwt:Secret"] ?? "N8YkceBz91uYzftL6u03boBoKyzGmTtiP/d8gGMEMMU=";
            _jwtLifespanMinutes = int.TryParse(config["Jwt:LifespanMinutes"], out var mins) ? mins : 60;
        }

        public LoginResponse Login(string userName, string password)
        {
            if (!_userRepository.ValidateCredentials(userName, password))
            {
                _logger.LogWarning("Failed login attempt for {userName}", userName);
                return null;
            }

            var user = _userRepository.GetByUserName(userName);
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSecret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddMinutes(_jwtLifespanMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwt = tokenHandler.WriteToken(token);

            _logger.LogInformation("User {userName} logged in", userName);

            return new LoginResponse
            {
                Token = jwt,
                ExpiresAt = tokenDescriptor.Expires.Value
            };
        }

        public void Logout(string token)
    {
        // Add the token to the blacklist with its expiration time
        var principal = GetPrincipalFromToken(token);
        if (principal?.FindFirst("exp") is Claim expClaim &&
            long.TryParse(expClaim.Value, out var expUnix))
        {
            var expiry = DateTimeOffset.FromUnixTimeSeconds(expUnix).UtcDateTime;
            _revokedTokens[token] = expiry;
        }
        _logger.LogInformation("User logged out (token: {Token})", token);
    }

    public bool IsAuthenticated(string token)
    {
        // Check if token is blacklisted
        if (_revokedTokens.TryGetValue(token, out var expiry))
        {
            if (DateTime.UtcNow < expiry)
                return false; // Token is revoked and not yet expired
            else
                _revokedTokens.TryRemove(token, out _); // Cleanup expired token
        }
        var principal = GetPrincipalFromToken(token);
        return principal != null;
    }

        public User? GetUserFromToken(string token)
        {
            var principal = GetPrincipalFromToken(token);
            if (principal == null) return null;
            var userName = principal.Identity.Name;
            return _userRepository.GetByUserName(userName);
        }

        private ClaimsPrincipal GetPrincipalFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var key = Encoding.ASCII.GetBytes(_jwtSecret);
                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out _);
                return principal;
            }
            catch
            {
                return null;
            }
        }
    }
}
