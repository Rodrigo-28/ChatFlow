using ChatFlow.Application.DTOs.Responses;
using ChatFlow.Application.Interfaces;
using ChatFlow.Domain.Interfaces;
using ChatFlow.Domain.Models;

namespace ChatFlow.Application.Services
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly IRefreshTokenRepository _refreshRepo;
        private readonly IRefreshTokenProvider _refreshProvider;
        private readonly IJwtTokenService _jwtTokenService;

        // Vida útil del refresh token (ajustable luego por config si querés)
        private static readonly TimeSpan _refreshLifetime = TimeSpan.FromDays(7);

        public RefreshTokenService(
            IRefreshTokenRepository refreshRepo,
            IRefreshTokenProvider refreshProvider,
            IJwtTokenService jwtTokenService)
        {
            _refreshRepo = refreshRepo;
            _refreshProvider = refreshProvider;
            _jwtTokenService = jwtTokenService;
        }


        public async Task<TokenPairDto> IssueOnLoginAsync(User user)
        {

            var accessToken = _jwtTokenService.GenerateJwtToken(user);


            var plain = _refreshProvider.GeneratePlainToken();
            var hash = _refreshProvider.ComputeSha256(plain);

            // 3) Persistir el refresh token (SOLO hash en DB)
            var entity = new RefreshToken
            {
                UserId = user.Id,
                TokenHash = hash,
                CreatedAtUtc = DateTime.UtcNow,
                ExpiresAtUtc = DateTime.UtcNow.Add(_refreshLifetime)
            };
            await _refreshRepo.CreateAsync(entity);

            // 4) Devolver par de tokens al cliente
            return new TokenPairDto
            {
                Token = accessToken,
                RefreshToken = plain
            };
        }


        public async Task<TokenPairDto> RefreshAsync(string refreshTokenPlain, bool rotate = true)
        {
            // 1) Convertir el valor plano a hash para buscar en DB
            var hash = _refreshProvider.ComputeSha256(refreshTokenPlain);

            // 2) Buscar el refresh token por hash (ideal que el repo incluya rt.User)
            var rt = await _refreshRepo.GetByHashAsync(hash);

            // 3) Validaciones mínimas de seguridad
            if (rt == null)
                throw new Exception("Refresh token not found.");

            if (!rt.IsActive)
                throw new Exception("Refresh token is expired or revoked.");


            if (rt.RevokedAtUtc != null || rt.ReplacedByTokenHash != null)
                throw new Exception("Refresh token has already been used.");


            if (rt.User == null)
                throw new Exception("Associated user not found.");
            var user = rt.User;

            // 5) Emitir nuevo access token
            var newAccess = _jwtTokenService.GenerateJwtToken(user);

            // 6) Si no queremos rotar (menos seguro), devolvemos el mismo refresh token
            if (!rotate)
            {
                return new TokenPairDto
                {
                    Token = newAccess,
                    RefreshToken = refreshTokenPlain
                };
            }


            var newPlain = _refreshProvider.GeneratePlainToken();
            var newHash = _refreshProvider.ComputeSha256(newPlain);

            rt.RevokedAtUtc = DateTime.UtcNow;
            rt.ReasonRevoked = "rotation";
            rt.ReplacedByTokenHash = newHash;
            await _refreshRepo.UpdateAsync(rt);

            var newRt = new RefreshToken
            {
                UserId = rt.UserId,
                TokenHash = newHash,
                CreatedAtUtc = DateTime.UtcNow,
                ExpiresAtUtc = DateTime.UtcNow.Add(_refreshLifetime)
            };
            await _refreshRepo.CreateAsync(newRt);

            return new TokenPairDto
            {
                Token = newAccess,
                RefreshToken = newPlain
            };
        }


        public async Task RevokeAsync(string refreshTokenPlain, string reason)
        {
            var hash = _refreshProvider.ComputeSha256(refreshTokenPlain);
            var rt = await _refreshRepo.GetByHashAsync(hash);
            if (rt == null) return;
            if (rt.RevokedAtUtc != null) return;

            rt.RevokedAtUtc = DateTime.UtcNow;
            rt.ReasonRevoked = reason;
            await _refreshRepo.UpdateAsync(rt);
        }


        public async Task RevokeAllForUserAsync(Guid userId, string reason)
        {
            await _refreshRepo.RevokeAllForUserAsync(userId, reason);

        }
    }
}
