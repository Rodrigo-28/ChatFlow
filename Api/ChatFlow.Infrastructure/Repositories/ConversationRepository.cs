using ChatFlow.Domain.Interfaces;
using ChatFlow.Domain.Models;
using ChatFlow.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace ChatFlow.Infrastructure.Repositories
{
    public class ConversationRepository : IConversationRepository
    {
        private readonly ApplicationDbContext _context;

        public ConversationRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Conversation?> GetOne(Guid receiverId, Guid senderId)
        {
            return await _context.Conversation
                .Include(c => c.Messages.OrderBy(m => m.SentAt))
                .FirstOrDefaultAsync(c =>
            c.User1Id == receiverId && c.User2Id == senderId ||
            c.User1Id == senderId && c.User2Id == receiverId);
        }
        public async Task<Conversation> Create(Conversation conversation)
        {
            _context.Conversation.Add(conversation);
            await _context.SaveChangesAsync();
            return conversation;
        }
    }
}
