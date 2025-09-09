using ChatFlow.Application.DTOs.Requests;
using ChatFlow.Application.DTOs.Responses;
using ChatFlow.Application.Exceptions;
using ChatFlow.Application.Interfaces;
using ChatFlow.Domain.Enums;
using ChatFlow.Domain.Interfaces;

namespace ChatFlow.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserService _userService;
        private readonly IPasswordEncryptionService _passwordEncryptionService;
        private readonly IJwtTokenService _jwtTokenService;

        public AuthService(IUserService userService, IPasswordEncryptionService passwordEncryptionService, IJwtTokenService jwtTokenService)
        {
            _userService = userService;
            _passwordEncryptionService = passwordEncryptionService;
            _jwtTokenService = jwtTokenService;
        }

        public async Task<LoginResponseDto> Login(LoginRequestDto userDto)
        {
            var user = await _userService.GetOne(u => u.Email == userDto.Email);

            if (user == null)
            {
                throw new BadRequestException("Invalid Password")
                {
                    ErrorCode = "001"
                };
            }

            //Check password
            var isValid = _passwordEncryptionService.VerifyPassword(user.Password, userDto.Password);



            if (!isValid)
            {
                throw new BadRequestException("Invalid Password")
                {
                    ErrorCode = "001"
                };
            }

            //Generate token
            var token = _jwtTokenService.GenerateJwtToken(user);

            return new LoginResponseDto { Token = token };
        }

        public async Task<UserResponseDto> Register(RegisterDto body)
        {
            var user = await _userService.GetOneByEmailAndUsername(body.Email, body.Username);

            if (user != null)
            {
                throw new BadRequestException("User already exists");
            }
            CreateUserDto payload = new CreateUserDto
            {
                Username = body.Username,
                Email = body.Email,
                Password = body.Password,
                RoleId = (int)UserRole.User
            };
            return await _userService.Create(payload);
        }

        public async Task<CurrentUserResponseDto> GetMe(Guid senderId)
        {
            var user =  await _userService.GetOne(u => u.Id == senderId);
            return new CurrentUserResponseDto { senderName = user.Username, senderId = user.Id};
            
        }
    }
}
