using Microsoft.AspNetCore.Mvc;
using StoreManagementAPI.Middlewares;
using StoreManagementAPI.Services;

namespace StoreManagementAPI.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : Controller
    {
        private readonly UserService _userService;

        public AuthController(UserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest body)
        {
            if(string.IsNullOrEmpty(body.Username) || string.IsNullOrEmpty(body.Password))
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

            //string token = JWTTokenService.GenerateToken(user);
            //Response.Headers.Add("Authorization", "Bearer " + token);


            return Ok(new
            {
                message = "Login success",
                user = user,
            });
        }



    }

    public class LoginRequest
    {
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
    }
}
