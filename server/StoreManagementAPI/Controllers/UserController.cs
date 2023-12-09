using Microsoft.AspNetCore.Mvc;
using StoreManagementAPI.Services;

namespace StoreManagementAPI.Controllers
{
    [ApiController]
    public class UserController : Controller
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [Route("admin/users")]
        public async Task<IActionResult> Get()
        {
            var users = await _userService.Get();
            return Ok(new { message = "Get users success", users = users });
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


        // TODO: Implement create user
        [HttpPost]
        [Route("admin/users/create")]
        public async Task<IActionResult> Create(string email, string role)
        {
            if(string.IsNullOrEmpty(email) || string.IsNullOrEmpty(role))
                return BadRequest(new { message = "Please fill all fields" });

            return null;
        }


    }
}
