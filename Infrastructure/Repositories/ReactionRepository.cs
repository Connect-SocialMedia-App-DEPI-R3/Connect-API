using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ReactionRepository : IReactionRepository
{
    private readonly AppDbContext _context;
    
    public ReactionRepository(AppDbContext context) => _context = context;

    public Task<Reaction?> GetReactionAsync(Guid userId, Guid postId) =>
        _context.Reactions
            .FirstOrDefaultAsync(r => r.UserId == userId && r.PostId == postId);

    public Task<List<Reaction>> GetPostReactionsAsync(Guid postId) =>
        _context.Reactions
            .Include(r => r.User)
            .Where(r => r.PostId == postId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

    public Task<int> GetPostReactionCountAsync(Guid postId) =>
        _context.Reactions
            .CountAsync(r => r.PostId == postId);

    public Task<bool> HasUserReactedAsync(Guid userId, Guid postId) =>
        _context.Reactions
            .AnyAsync(r => r.UserId == userId && r.PostId == postId);

    public async Task AddAsync(Reaction reaction) =>
        await _context.Reactions.AddAsync(reaction);

    public Task DeleteAsync(Reaction reaction)
    {
        _context.Reactions.Remove(reaction);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync() =>
        _context.SaveChangesAsync();
}