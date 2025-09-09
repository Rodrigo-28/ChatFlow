using ChatFlow.Domain.Models;

namespace ChatFlow.Domain.Interfaces
{
    public interface IConversationRepository
    {
        Task<Conversation?> GetOne(Guid receiverId, Guid senderId);
        Task<Conversation> Create(Conversation conversation);
    }
}
