using ChatFlow.Application.DTOs.Requests;
using ChatFlow.Application.DTOs.Responses;
using ChatFlow.Domain.Models;
using System.Linq.Expressions;

namespace ChatFlow.Application.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserResponseDto>> GetAllExceptSenderUser(Guid senderId);
        Task<UserResponseDto> GetOne(Guid userId);
        Task<UserResponseDto> Create(CreateUserDto createUserDto);

        Task<UserResponseDto> Update(Guid userId, UpdateUserDto updateUserDto);
        Task<GenericResponseDto> Delete(Guid userId);
        Task<User> GetOne(Expression<Func<User, bool>> predicate);
        Task<User> GetOneByEmailAndUsername(string email, string username);



    }
}
