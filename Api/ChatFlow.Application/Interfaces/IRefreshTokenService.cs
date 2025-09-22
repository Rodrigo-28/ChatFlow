using ChatFlow.Application.DTOs.Responses;
using ChatFlow.Domain.Models;

namespace ChatFlow.Application.Interfaces
{
    public interface IRefreshTokenService
    {
        Task<TokenPairDto> IssueOnLoginAsync(User user);
        Task<TokenPairDto> RefreshAsync(string refreshTokenPlain, bool rotate = true);
        Task RevokeAsync(string refreshTokenPlain, string reason);
        Task RevokeAllForUserAsync(Guid userId, string reason);
    }
}
