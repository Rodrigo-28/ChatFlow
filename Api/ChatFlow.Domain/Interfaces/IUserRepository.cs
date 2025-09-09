using ChatFlow.Domain.Models;
using System.Linq.Expressions;

namespace ChatFlow.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllExceptSenderUser(Guid senderId);
        Task<User?> GetOne(Guid userId);
        Task<User> Create(User user);
        Task<User> Update(User user);
        Task<User?> GetOne(Expression<Func<User, bool>> predicate);
        Task<bool> Delete(User user);
    }
}
