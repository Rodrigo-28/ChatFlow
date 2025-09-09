using ChatFlow.Application.DTOs.Requests;
using ChatFlow.Application.DTOs.Responses;

namespace ChatFlow.Application.Interfaces
{
    public interface IAuthService
    {
        public Task<LoginResponseDto> Login(LoginRequestDto userDto);
        public Task<UserResponseDto> Register(RegisterDto body);
        public Task<CurrentUserResponseDto> GetMe(Guid senderId);
    }
}
