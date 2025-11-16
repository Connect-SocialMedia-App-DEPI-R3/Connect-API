using Domain.Entities;

namespace Domain.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(string id);
    Task<List<User>> GetAllAsync();
    Task AddAsync(User user);
    Task SaveChangesAsync();
}
