using Domain.Entities;

namespace Domain.Interfaces;

public interface IPostRepository
{
    Task<Post?> GetByIdAsync(Guid id);
    Task<List<Post>> GetAllAsync();
    Task AddAsync(Post post);
    Task UpdateAsync(Post post);
    Task DeleteAsync(Post post);
    Task SaveChangesAsync();
}