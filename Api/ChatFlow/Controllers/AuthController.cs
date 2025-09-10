using ChatFlow.Application.DTOs.Requests;
using ChatFlow.Application.Interfaces;
using ChatFlow.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatFlow.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto userDto)
        {


            var token = await _authService.Login(userDto);

            return Ok(token);
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto body)
        {

            var user = await _authService.Register(body);

            return Ok(user);
        }
        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetMe()
        {
            Guid senderId = UserHelper.GetUserId(User);
            var response = await _authService.GetMe(senderId);
            return Ok(response);
        }
    }
}
