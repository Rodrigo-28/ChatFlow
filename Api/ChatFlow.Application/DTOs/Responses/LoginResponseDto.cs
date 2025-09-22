namespace ChatFlow.Application.DTOs.Responses
{
    public class LoginResponseDto
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; } = default!;
    }
}
