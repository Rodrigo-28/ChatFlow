namespace ChatFlow.Application.DTOs.Responses
{
    public class TokenPairDto
    {
        public string Token { get; set; } = default!;          // access token (JWT)
        public string RefreshToken { get; set; } = default!;   // refresh token (valor p
    }
}
