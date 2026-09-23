using WorldSkillsAPI.Interfaces;
using WorldSkillsAPI.Requests;
using Microsoft.AspNetCore.Mvc;

namespace WorldSkillsAPI.Controllers
{
    [ApiController]
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        [Route("RegUser")]
        public async Task<IActionResult> RegUser([FromBody] UserRequest newUser)
        {
            return await _userService.RegisterAsync(newUser);
        }

        [HttpPost]
        [Route("AuthUser")]
        public async Task<IActionResult> AuthUser([FromBody] AuthUserRequest loginData)
        {
            return await _userService.LoginAsync(loginData);
        }

        [HttpPost]
        [Route("Logout")]
        [RoleAuthorized]
        public async Task<IActionResult> Logout()
        {
            var token = Request.Headers["Authorization"].FirstOrDefault();
            return await _userService.LogoutAsync(token);
        }

        [HttpGet]
        [Route("Profile")]
        [RoleAuthorized]
        public async Task<IActionResult> Profile()
        {
            var token = Request.Headers["Authorization"].FirstOrDefault();
            return await _userService.GetProfileAsync(token);
        }
    }
}