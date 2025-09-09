using ChatFlow.Application.Interfaces;
using ChatFlow.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace ChatFlow.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConversationsController : ControllerBase
    {
        private readonly IConversationService _conversationService;

        public ConversationsController(IConversationService conversationService)
        {
            _conversationService = conversationService;
        }

        [HttpGet("{receiverId}")]
        public async Task<IActionResult> GetConversation(Guid receiverId)
        {
            var senderId = UserHelper.GetUserId(User);
                
            
            var conversation = await _conversationService.GetOneOrCreate(receiverId, senderId);

            return Ok(conversation);
        }

    }
}
