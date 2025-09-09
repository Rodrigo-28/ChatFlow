using ChatFlow.Domain.Models;

namespace ChatFlow.Domain.Interfaces
{
    public interface IJwtTokenService
    {
        public string GenerateJwtToken(User user);
    }
}
