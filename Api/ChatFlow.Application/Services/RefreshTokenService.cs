using ChatFlow.Application.DTOs.Responses;
using ChatFlow.Application.Interfaces;
using ChatFlow.Domain.Interfaces;
using ChatFlow.Domain.Models;

namespace ChatFlow.Application.Services
{
    /// <summary>
    /// Maneja la emisión, validación, rotación y revocación de refresh tokens.
    /// - Guarda en DB SOLO el hash del refresh token.
    /// - Devuelve al cliente el valor plano.
    /// </summary>
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly IRefreshTokenRepository _refreshRepo;      // Acceso a tabla RefreshTokens
        private readonly IRefreshTokenProvider _refreshProvider;    // Genera tokens aleatorios y calcula hash
        private readonly IJwtTokenService _jwtTokenService;         // Emite access tokens (JWT)

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

        /// <summary>
        /// Se usa en LOGIN:
        /// - Emite access token (JWT con claim "ver").
        /// - Crea refresh token (valor plano + hash guardado en DB).
        /// - Devuelve ambos al cliente.
        /// </summary>
        public async Task<TokenPairDto> IssueOnLoginAsync(User user)
        {
            // 1) Crear access token (30 min aprox.; tu JwtTokenService ya incluye claim "ver")
            var accessToken = _jwtTokenService.GenerateJwtToken(user);

            // 2) Crear refresh token (valor plano que devolveremos + hash que guardaremos)
            var plain = _refreshProvider.GeneratePlainToken();   // p.ej. base64 de 64 bytes aleatorios
            var hash = _refreshProvider.ComputeSha256(plain);   // hash seguro

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

        /// <summary>
        /// Se usa en /auth/refresh:
        /// - Valida el refresh token entrante.
        /// - Emite nuevo access token.
        /// - (Por defecto) Rota el refresh: invalida el viejo y crea uno nuevo.
        /// </summary>
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

            // Defensa anti-reuso básico:
            // si ya fue rotado (ReplacedByTokenHash set) o revocado, no permitir su uso otra vez
            if (rt.RevokedAtUtc != null || rt.ReplacedByTokenHash != null)
                throw new Exception("Refresh token has already been used.");

            // 4) Necesitamos el usuario asociado para crear el nuevo access token
            // (Versión EXPLÍCITA y legible, sin '?? throw')
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

            // 7) Rotación segura:
            //    - Generar nuevo refresh (plain + hash)
            //    - Marcar el viejo como revocado, con referencia al nuevo (para detectar re-uso)
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

            // 8) Devolver nuevo access + NUEVO refresh
            return new TokenPairDto
            {
                Token = newAccess,
                RefreshToken = newPlain
            };
        }

        /// <summary>
        /// Logout de ESTE dispositivo/sesión:
        /// - Revoca SOLO el refresh token pasado (si está activo).
        /// - No toca otros dispositivos.
        /// </summary>
        public async Task RevokeAsync(string refreshTokenPlain, string reason)
        {
            var hash = _refreshProvider.ComputeSha256(refreshTokenPlain);
            var rt = await _refreshRepo.GetByHashAsync(hash);
            if (rt == null) return;                 // nada que hacer
            if (rt.RevokedAtUtc != null) return;    // ya estaba revocado

            rt.RevokedAtUtc = DateTime.UtcNow;
            rt.ReasonRevoked = reason;              // p.ej. "logout"
            await _refreshRepo.UpdateAsync(rt);
        }

        /// <summary>
        /// Logout GLOBAL (todas las sesiones):
        /// - Revoca todos los refresh tokens activos del usuario.
        /// - (Sugerido fuera de este servicio) Incrementar TokenVersion del usuario para invalidar access tokens al instante.
        /// </summary>
        public async Task RevokeAllForUserAsync(Guid userId, string reason)
        {
            await _refreshRepo.RevokeAllForUserAsync(userId, reason);
            // Nota: en tu AuthService o UserService podés hacer: user.TokenVersion++ y persistir,
            // para que los access tokens queden inválidos por versión.
        }
    }
}
