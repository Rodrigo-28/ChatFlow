using ChatFlow.Domain.Interfaces;
using ChatFlow.Domain.Models;
using ChatFlow.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace ChatFlow.Infrastructure.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly ApplicationDbContext _context;

        public MessageRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Message> Create(Message message)
        {
            _context.Message.Add(message);
            await _context.SaveChangesAsync();
            return message;
        }

        public async Task MarkAsRead(Guid conversationId, Guid senderId)
        {
            var messages = await _context.Message.Where(m => m.ConversationId == conversationId && m.SenderId != senderId && !m.IsRead).ToListAsync();

            if (messages.Any())
            {
                foreach (var message in messages)
                {
                    message.IsRead = true;
                }

                await _context.SaveChangesAsync();
            }
        }
    }
}
