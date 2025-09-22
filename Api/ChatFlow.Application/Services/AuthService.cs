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
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly IUserRepository _userRepository;

        public AuthService(IUserRepository userRepository, IUserService userService, IPasswordEncryptionService passwordEncryptionService, IJwtTokenService jwtTokenService, IRefreshTokenService refreshTokenService)
        {
            _userService = userService;
            _passwordEncryptionService = passwordEncryptionService;
            _jwtTokenService = jwtTokenService;
            _refreshTokenService = refreshTokenService;
            _userRepository = userRepository;
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

            // 3) Emitir par de tokens (access + refresh) y persistir el refresh (hash) en DB
            var pair = await _refreshTokenService.IssueOnLoginAsync(user);
            // 4) Devolver en el mismo DTO de siempre (compatibilidad con el front)
            return new LoginResponseDto
            {
                Token = pair.Token,                  // access token (JWT)
                RefreshToken = pair.RefreshToken     // refresh token (valor plano)
            };
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
            var user = await _userService.GetOne(u => u.Id == senderId);
            return new CurrentUserResponseDto { senderName = user.Username, senderId = user.Id };

        }

        public async Task<LoginResponseDto> RefreshAsync(RefreshTokenRequestDto body)
        {
            if (string.IsNullOrWhiteSpace(body.RefreshToken))
                throw new Exception("Refresh token is required.");
            // Emite nuevo access y rota el refresh por seguridad
            var pair = await _refreshTokenService.RefreshAsync(body.RefreshToken, rotate: true);
            return new LoginResponseDto
            {
                Token = pair.Token,
                RefreshToken = pair.RefreshToken
            };
        }

        public async Task<GenericResponseDto> LogoutAsync(RefreshTokenRequestDto body)
        {
            if (string.IsNullOrWhiteSpace(body.RefreshToken))
                throw new BadRequestException("Refresh token is required");
            await _refreshTokenService.RevokeAsync(body.RefreshToken, reason: "logout");
            return new GenericResponseDto { Success = true };
        }

        public async Task<GenericResponseDto> LogoutAllAsync(Guid userId)
        {
            // 1) Revoca todos los refresh tokens activos del usuario
            await _refreshTokenService.RevokeAllForUserAsync(userId, reason: "logout_all");
            // 2) Sube TokenVersion → invalida TODOS los access tokens emitidos previamente
            var user = await _userRepository.GetOne(userId);
            if (user == null)
                throw new BadRequestException("User not found");
            user.TokenVersion += 1;
            await _userRepository.Update(user);

            return new GenericResponseDto { Success = true };
        }
    }
}
