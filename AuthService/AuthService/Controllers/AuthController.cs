using AuthService.API.Models;
using AuthService.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var response = _authService.Login(request.UserName, request.Password);
            if (response == null)
                return Unauthorized("Invalid credentials");
            return Ok(response);
        }

        [Authorize]
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            _authService.Logout(token);
            return Ok("Logged out");
        }

        [Authorize]
        [HttpGet("profile")]
        public IActionResult GetUserProfile()
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var user = _authService.GetUserFromToken(token);
            if (user == null)
                return Unauthorized();
            return Ok(new { user.UserName, user.Role });
        }
    }
}
