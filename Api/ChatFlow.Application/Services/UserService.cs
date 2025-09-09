using AutoMapper;
using ChatFlow.Application.DTOs.Requests;
using ChatFlow.Application.DTOs.Responses;
using ChatFlow.Application.Interfaces;
using ChatFlow.Domain.Interfaces;
using ChatFlow.Domain.Models;
using System.Linq.Expressions;

namespace ChatFlow.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IPasswordEncryptionService _passwordEncryptionService;

        public UserService(IUserRepository userRepository, IMapper mapper, IPasswordEncryptionService passwordEncryptionService)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _passwordEncryptionService = passwordEncryptionService;
        }
        public async Task<UserResponseDto> Create(CreateUserDto createUserDto)
        {
            var userExist = await _userRepository.GetOne(u => u.Email == createUserDto.Email);
            if (userExist != null)
            {
                throw new Exception();
            }
            createUserDto.Password = _passwordEncryptionService.HashPassword(createUserDto.Password);
            var newUser = _mapper.Map<User>(createUserDto);

            var user = await _userRepository.Create(newUser);
            return _mapper.Map<UserResponseDto>(user);


        }

        public async Task<GenericResponseDto> Delete(Guid userId)
        {
            var user = await _userRepository.GetOne(userId);
            if (user == null)
            {
                throw new Exception();
            };
            await _userRepository.Delete(user);
            return new GenericResponseDto { Success = true };
        }

        public async Task<IEnumerable<UserResponseDto>> GetAllExceptSenderUser(Guid senderId)
        {
            var users = await _userRepository.GetAllExceptSenderUser(senderId);
            return _mapper.Map<IEnumerable<UserResponseDto>>(users);
        }

        public async Task<UserResponseDto> GetOne(Guid userId)
        {
            var user = await _userRepository.GetOne(userId);

            if (user == null)
            {
                throw new Exception();
            }
            return _mapper.Map<UserResponseDto>(user);
        }

        public async Task<User> GetOne(Expression<Func<User, bool>> predicate)
        {
            var user = await _userRepository.GetOne(predicate);

            if (user == null)
            {
                throw new Exception();
            }
            return _mapper.Map<User>(user);
        }
        public async Task<User?> GetOneByEmailAndUsername(string email, string username)
        {
            var user = await _userRepository.GetOne(u => u.Email == email || u.Username == username);

            return _mapper.Map<User>(user);
        }

        public async Task<UserResponseDto> Update(Guid userId, UpdateUserDto updateUserDto)
        {
            var user = await _userRepository.GetOne(userId);
            if (user == null)
            {
                throw new Exception();
            }
            var updatedUser = _mapper.Map(updateUserDto, user);
            updatedUser.Password = _passwordEncryptionService.HashPassword(updatedUser.Password);
            await _userRepository.Update(updatedUser);
            return _mapper.Map<UserResponseDto>(updatedUser);

        }
    }
}
