using ChatFlow.Application.DTOs.Requests;
using ChatFlow.Application.Interfaces;
using ChatFlow.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace ChatFlow.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly IMessageService _messageService;
        public MessagesController(IMessageService messageService)
        {
            _messageService = messageService;
        }
        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageDto body)
        {
            Guid senderId = UserHelper.GetUserId(User);
            var user = await _messageService.SendMessage(body, senderId);

            return Ok(user);
        }
    }
}
