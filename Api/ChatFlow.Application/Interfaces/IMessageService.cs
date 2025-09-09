using ChatFlow.Application.DTOs.Requests;
using ChatFlow.Application.DTOs.Responses;

namespace ChatFlow.Application.Interfaces
{
    public interface IMessageService
    {
        public Task<SendMessageResponseDto> SendMessage(SendMessageDto body, Guid senderId);
        Task MarkAsRead(Guid conversationId, Guid senderId);

    }
}
