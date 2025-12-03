using Domain.Entities;

namespace Domain.Interfaces;

public interface IReactionRepository
{
    Task<Reaction?> GetReactionAsync(Guid userId, Guid postId);
    Task<List<Reaction>> GetPostReactionsAsync(Guid postId);
    Task<int> GetPostReactionCountAsync(Guid postId);
    Task<bool> HasUserReactedAsync(Guid userId, Guid postId);
    Task AddAsync(Reaction reaction);
    Task DeleteAsync(Reaction reaction);
    Task SaveChangesAsync();
}