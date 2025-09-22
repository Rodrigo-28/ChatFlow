using ChatFlow.Domain.Models;

namespace ChatFlow.Domain.Interfaces
{
    public interface IRefreshTokenRepository
    {
        //CreateAsync: guardar un refresh token nuevo.
        Task<RefreshToken> CreateAsync(RefreshToken token);
        //GetByHashAsync: buscar por TokenHash (cuando te llega en /auth/refresh).
        Task<RefreshToken?> GetByHashAsync(string tokenHash);
        //  GetActiveByUserAsync: listar los tokens activos de un usuario(útil para limpieza/diagnóstico).
        Task<IEnumerable<RefreshToken>> GetActiveByUserAsync(Guid userId);
        //UpdateAsync: para marcar revocado o rotado.
        Task<RefreshToken> UpdateAsync(RefreshToken token);
        //RevokeAllForUserAsync: logout global rápido.
        Task RevokeAllForUserAsync(Guid userId, string reason);
    }
}
