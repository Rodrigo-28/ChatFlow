using ChatFlow.Domain.Interfaces;
using ChatFlow.Domain.Models;
using ChatFlow.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace ChatFlow.Infrastructure.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly ApplicationDbContext _context;

        public RefreshTokenRepository(ApplicationDbContext context)
        {
            this._context = context;
        }
        public async Task<RefreshToken> CreateAsync(RefreshToken token)
        {
            _context.RefreshTokens.Add(token);
            await _context.SaveChangesAsync();
            return token;
        }

        public async Task<IEnumerable<RefreshToken>> GetActiveByUserAsync(Guid userId)
        {
            return await _context.RefreshTokens
                 .Where(rt => rt.UserId == userId
                 && rt.RevokedAtUtc == null
                 && rt.ExpiresAtUtc > DateTime.UtcNow
                 ).ToListAsync();
        }

        public async Task<RefreshToken?> GetByHashAsync(string tokenHash)
        {
            return await _context.RefreshTokens
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.TokenHash == tokenHash);
        }

        public async Task RevokeAllForUserAsync(Guid userId, string reason)
        {
            var now = DateTime.UtcNow;
            var tokens = await _context.RefreshTokens
                .Where(rt => rt.UserId == userId && rt.RevokedAtUtc == null)
                .ToListAsync();
            foreach (var t in tokens)
            {
                t.RevokedAtUtc = now;
                t.ReasonRevoked = reason;
            }
            await _context.SaveChangesAsync();
        }

        public async Task<RefreshToken> UpdateAsync(RefreshToken token)
        {
            _context.RefreshTokens.Update(token);
            await _context.SaveChangesAsync();
            return token;

        }
    }
}
