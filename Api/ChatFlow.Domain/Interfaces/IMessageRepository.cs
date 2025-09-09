using ChatFlow.Domain.Models;

namespace ChatFlow.Domain.Interfaces
{
    public interface IMessageRepository
    {
        Task<Message> Create(Message message);
        Task MarkAsRead(Guid conversationId, Guid senderId);
    }
}
