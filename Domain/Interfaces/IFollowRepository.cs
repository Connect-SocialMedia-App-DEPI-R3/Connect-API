using Domain.Entities;

namespace Domain.Interfaces;

public interface IFollowRepository
{
    Task<Follow?> GetFollowAsync(Guid followerId, Guid followingId);
    Task<List<Follow>> GetFollowersAsync(Guid userId);
    Task<List<Follow>> GetFollowingAsync(Guid userId);
    Task<bool> IsFollowingAsync(Guid followerId, Guid followingId);
    Task AddAsync(Follow follow);
    Task DeleteAsync(Follow follow);
    Task SaveChangesAsync();
}
