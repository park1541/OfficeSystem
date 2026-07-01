using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeSystem.DTOs;
using OfficeSystem.Services;

namespace OfficeSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService; 
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody]RegisterRequest request) 
        {
            var user = await _userService.Register(request);
            if (user == null)
                return Conflict("이미 존재하는 아이디입니다.");
            var response = new UserResponse
            {
                LoginId = user.LoginId,
                Name = user.Name,
                EmployeeNumber = user.EmployeeNumber,
                RoleId = user.RoleId
            };
            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody]LoginRequest login)
        {
            var result = await _userService.Login(login);
            if(result == null)
            {
                return Unauthorized("아이디 혹은 비밀번호가 일치하지 않습니다.");
            }

            var response = new UserResponse
            {
                LoginId = result.User.LoginId,
                Name = result.User.Name,
                EmployeeNumber = result.User.EmployeeNumber,
                RoleId = result.User.RoleId
            };
            return Ok(new {token = result.Token, user = response});
        }
        [HttpGet("public")]
        public IActionResult PublicTest()
        {
            return Ok("아무나 볼 수 있음");
        }

        [Authorize]
        [HttpGet("secret")]
        public IActionResult SecretTest()
        {
            return Ok("토큰 있어야 볼 수 있음");
        }
    }
}

