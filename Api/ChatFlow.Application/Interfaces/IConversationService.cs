using ChatFlow.Application.DTOs.Responses;

namespace ChatFlow.Application.Interfaces
{
    public interface IConversationService
    {
        Task<ConversationResultDto> GetOneOrCreate(Guid receiverId, Guid senderId);
    }
}
