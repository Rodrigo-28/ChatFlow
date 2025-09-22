namespace ChatFlow.Domain.Models
{
    public class RefreshToken
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        // Seguridad: guardamos hash (p.ej. SHA256) del token
        public string TokenHash { get; set; } = default!;

        public DateTime ExpiresAtUtc { get; set; }
        public DateTime CreatedAtUtc { get; set; }



        public DateTime? RevokedAtUtc { get; set; }
        public string? ReasonRevoked { get; set; }

        // Para rotación: referencia al hash del token que lo reemplaza
        public string? ReplacedByTokenHash { get; set; }

        public User User { get; set; } = default!;

        // Helpers de estado
        public bool IsExpired => DateTime.UtcNow >= ExpiresAtUtc;
        public bool IsActive => RevokedAtUtc is null && !IsExpired;

    }
}
