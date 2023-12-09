using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using StoreManagementAPI.Middlewares;
using StoreManagementAPI.Models;
using StoreManagementAPI.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace StoreManagementAPI.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : Controller
    {
        private readonly UserService _userService;
        private readonly JWTTokenService _tokenService;

        public AuthController(UserService userService, JWTTokenService tokenService)
        {
            _userService = userService;
            _tokenService = tokenService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest body)
        {
            if (string.IsNullOrEmpty(body.Username) || string.IsNullOrEmpty(body.Password))
                return BadRequest(new { message = "Please fill in all fields!" });


            string username = body.Username;
            string password = body.Password;

            var user = await _userService.GetByUsername(username);
            if (user == null || !PasswordService.VerifyPassword(password, user.Password))
                return BadRequest(new { message = "Invalid credentials!" });

            if (user.Status.ToString().Equals("LOCKED"))
                return BadRequest(new { message = "Please contact admin to unlock your account!" });

            if (user.Username.Equals(password))
                return BadRequest(new { message = "Please contact admin to reset your password!" });

            string token = _tokenService.GenerateToken(user);
            Response.Headers["Authorization"] = "Bearer " + token;

            return Ok(new
            {
                message = "Login success",
                user = user,
            });
        }

        [HttpPost("validate")]
        public async Task<IActionResult> Validate([FromHeader(Name = "Authorization")] string token, [FromBody] Dictionary<string, string> body)
        {
            string resource = body["resource"];

            token = token.Substring(7);
            string? email = _tokenService.ValidateToken(token);
            if (email == null)
                return BadRequest(new { message = "Invalid token" });

            User user = await _userService.GetByEmail(email);

            if (user == null)
                return BadRequest(new { message = "Invalid token" });

            if (resource.Equals("ADMIN", StringComparison.OrdinalIgnoreCase))
            {
                if (!user.Role.Equals("ADMIN", StringComparison.OrdinalIgnoreCase) && !user.Role.Equals("OWNER", StringComparison.OrdinalIgnoreCase))
                    return StatusCode(StatusCodes.Status403Forbidden, new { message = "No permission" });
               
            }
            else if (resource.Equals("OWNER", StringComparison.OrdinalIgnoreCase))
            {
                if (!user.Role.Equals("OWNER", StringComparison.OrdinalIgnoreCase))
                    return StatusCode(StatusCodes.Status403Forbidden, new { message = "No permission" });
            }

            return Ok(new { message = "Valid token", user });
        }

    }
    public class LoginRequest
    {
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
    }
}
