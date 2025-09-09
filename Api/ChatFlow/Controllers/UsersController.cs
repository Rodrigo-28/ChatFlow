using ChatFlow.Application.DTOs.Requests;
using ChatFlow.Application.Interfaces;
using ChatFlow.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace ChatFlow.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UsersController> _logger;
        private readonly HttpClient _httpClient;
        public UsersController(IUserService userService, ILogger<UsersController> logger, HttpClient httpClient)
        {
            _userService = userService;
            _logger = logger;
            _httpClient = httpClient; // Injected HttpClient
        }
        [Authorize]
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetOne(Guid userId)
        {
            var user = await _userService.GetOne(userId);
            return Ok(user);
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserDto userDto)
        {
            //var validationResult = await validator.ValidateAsync(userDto);
            //if (!validationResult.IsValid)
            //{
            //    throw new BadRequestException(validationResult.ToString())
            //    {
            //        ErrorCode = "004"
            //    };
            //}


            var user = await _userService.Create(userDto);
            return Ok(user);
        }
        private async Task<string> GetCountryFromIpAsync(string ipAddress)
        {
            try
            {
                // Use ip-api.com to get geolocation data
                var response = await _httpClient.GetStringAsync($"http://ip-api.com/json/{ipAddress}");
                using var jsonDoc = JsonDocument.Parse(response);
                var root = jsonDoc.RootElement;

                // Check if the request was successful
                if (root.GetProperty("status").GetString() == "success")
                {
                    return root.GetProperty("country").GetString();
                }
                return "Unknown";
            }
            catch (Exception ex)
            {
                // Handle errors (e.g., API rate limit, network issues)
                // Log the error if needed
                return "Unknown";
            }
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var senderId = UserHelper.GetUserId(User);

            // Your existing logic
            var user = await _userService.GetAllExceptSenderUser(senderId);
            return Ok(user);
        }
        [HttpPut("{userId}")]
        public async Task<IActionResult> Update(Guid userId, [FromBody] UpdateUserDto userDto)
        {
            var user = await _userService.Update(userId, userDto);
            return Ok(user);
        }
        [HttpDelete("{userId}")]
        public async Task<IActionResult> Delete(Guid userId)
        {
            var userDeleted = await _userService.Delete(userId);

            return Ok(userDeleted);
        }


    }
}
