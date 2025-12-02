using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Infrastructure.Data;
namespace Infrastructure.Repositories;

public class PostRepository : IPostRepository
{
    private readonly AppDbContext _context;
    public PostRepository(AppDbContext context) => _context = context;

    public Task<Post?> GetByIdAsync(Guid id) =>
        _context.Posts
            .Include(p => p.User)
            .Include(p => p.Comments!)
                .ThenInclude(c => c.User)
            .Include(p => p.Reactions)
            .FirstOrDefaultAsync(p => p.Id == id);

    public Task<List<Post>> GetAllAsync() =>
        _context.Posts
            .Include(p => p.User)
            .Include(p => p.Comments)
            .Include(p => p.Reactions)
            .ToListAsync();

    public Task<List<Post>> GetByUsernameAsync(string username) =>
        _context.Posts
            .Include(p => p.User)
            .Include(p => p.Comments)
            .Include(p => p.Reactions)
            .Where(p => p.User.UserName == username && !p.User.IsDeleted)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

    public async Task AddAsync(Post post) =>
        await _context.Posts.AddAsync(post);

    public Task UpdateAsync(Post post)
    {
        _context.Posts.Update(post);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Post post)
    {
        _context.Posts.Remove(post);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync() =>
        _context.SaveChangesAsync();
}