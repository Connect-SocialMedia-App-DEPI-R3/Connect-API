using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class FollowRepository : IFollowRepository
{
    private readonly AppDbContext _context;
    
    public FollowRepository(AppDbContext context) => _context = context;

    public Task<Follow?> GetFollowAsync(Guid followerId, Guid followingId) =>
        _context.Follows
            .FirstOrDefaultAsync(f => f.FollowerId == followerId && f.FollowingId == followingId);

    public Task<List<Follow>> GetFollowersAsync(Guid userId) =>
        _context.Follows
            .Include(f => f.Follower)
            .Where(f => f.FollowingId == userId)
            .ToListAsync();

    public Task<List<Follow>> GetFollowingAsync(Guid userId) =>
        _context.Follows
            .Include(f => f.Following)
            .Where(f => f.FollowerId == userId)
            .ToListAsync();

    public Task<bool> IsFollowingAsync(Guid followerId, Guid followingId) =>
        _context.Follows
            .AnyAsync(f => f.FollowerId == followerId && f.FollowingId == followingId);

    public async Task AddAsync(Follow follow) =>
        await _context.Follows.AddAsync(follow);

    public Task DeleteAsync(Follow follow)
    {
        _context.Follows.Remove(follow);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync() =>
        _context.SaveChangesAsync();
}
