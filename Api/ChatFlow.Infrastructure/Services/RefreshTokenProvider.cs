using ChatFlow.Domain.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace ChatFlow.Infrastructure.Services
{
    public class RefreshTokenProvider : IRefreshTokenProvider
    {
        public string ComputeSha256(string plain)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(plain));
            return Convert.ToHexString(bytes);
        }

        public string GeneratePlainToken(int bytes = 64)
        {
            var buffer = new byte[bytes];
            RandomNumberGenerator.Fill(buffer);
            return Convert.ToBase64String(buffer);
        }
    }
}
