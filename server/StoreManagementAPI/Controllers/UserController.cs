using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoreManagementAPI.Middlewares;
using StoreManagementAPI.Models;
using StoreManagementAPI.Services;
using System.Text.RegularExpressions;

namespace StoreManagementAPI.Controllers
{
    [Route("api")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly UserService _userService;
        private readonly MailService _mailService;
        private readonly ResetPasswordTokenService _rPTService;

        public UserController(UserService userService, MailService mailService, ResetPasswordTokenService rPTService)
        {
            _rPTService = rPTService;
            _userService = userService;
            _mailService = mailService;
        }

        [HttpGet]
        [Route("admin/users")]
        public async Task<IActionResult> GetUsers([FromQuery(Name = "text")] string searchText = "")
        {
            IEnumerable<User> users;

            if (!string.IsNullOrEmpty(searchText))
                users = await _userService.SearchUser(searchText);
            else
                users = await _userService.GetAllUsers();

            foreach (var user in users)
                user.Password = "";

            return Ok(new { message = "Get users success", users });
        }

        [HttpGet]
        [Route("admin/users/{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var user = await _userService.GetById(id);
            if (user == null)
                return NotFound(new { message = "User not found" });

            user.Password = "";
            return Ok(new { message = "Get user success", user = user });
        }


        [HttpPost]
        [Route("admin/users/create")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest body)
        {
            try
            {
                string email = body.Email;
                string role = body.Role.ToUpper();

                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(role))
                    return BadRequest(new { message = "Please fill all fields" });

                string domain = _mailService.HOST.Split("smtp.")[1];
                if (!Regex.IsMatch(email, $"^\\w+@({domain})$"))
                    return BadRequest(new { message = "Invalid email or domain" });

                var existingUser = await _userService.GetByEmail(email);
                if (existingUser != null)
                    return BadRequest(new { message = "Email already exists" });

                if (!_userService.IsValidRole(role))
                    return BadRequest(new { message = "Invalid role" });

                if (role == "OWNER")
                    return BadRequest(new { message = "Cannot create owner" });

                string username = email.Split('@')[0];
                string password = PasswordService.HashPassword(username);

                User user = new User()
                {
                    Email = email,
                    Username = username,
                    Password = password,
                    Role = Enum.Parse<Role>(role),
                    Status = Status.NORMAL,
                };

                bool isAdded = await _userService.AddUser(user);

                if (!isAdded)
                    return BadRequest(new { message = "Create user failed" });

                var response = await ResetPassword(user.Id);
                if (response.StatusCode >= 400)
                    return response;

                return Ok(new
                {
                    message = "Create user success. A reset password mail has been sent to the user",
                    user
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Internal Server Error: {ex.Message}" });
            }
        }

        [HttpGet]
        [Route("admin/users/reset-password/{id}")]
        public async Task<ObjectResult> ResetPassword([FromRoute] string id)
        {
            try
            {
                Console.WriteLine($"Reset password for user {id}");
                User user = await _userService.GetById(id);
                if (user == null)
                    return BadRequest(new { message = "User not found" });

                string token = await _rPTService.CreateToken(id);
                if (token == null)
                    return BadRequest(new { message = "Create token failed" });

                if (!await _mailService.SendResetPasswordMail(user.Email, token))
                    return BadRequest(new { message = "Send reset password mail failed" });

                return Ok(new { message = "Send reset password mail success", user });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Internal Server Error: {ex.Message}" });
            }
        }

    }

    public class CreateUserRequest
    {
        public string Email { get; set; } = "";
        public string Role { get; set; } = "";
    }
}
