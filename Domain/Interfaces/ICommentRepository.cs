using Domain.Entities;

namespace Domain.Interfaces;

public interface ICommentRepository
{
    Task<Comment?> GetByIdAsync(Guid id);
    Task<List<Comment>> GetAllAsync();
    Task<List<Comment>> GetAllByPostIdAsync(Guid postId);
    Task AddAsync(Comment comment);
    Task UpdateAsync(Comment comment);
    Task DeleteAsync(Comment comment);
    Task SaveChangesAsync();
}