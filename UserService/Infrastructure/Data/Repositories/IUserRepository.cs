using Common.Interfaces;
using UserService.Domain.Entities;

namespace UserService.Infrastructure.Data.Repositories;

public interface IUserRepository : IBaseRepository<User>
{
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByEmailAsync(string email);
    Task AddAsync(User user);
    Task SaveChangesAsync();
}