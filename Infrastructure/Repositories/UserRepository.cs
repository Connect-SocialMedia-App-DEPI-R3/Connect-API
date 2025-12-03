using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;

namespace Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;
    public UserRepository(AppDbContext context) => _context = context;

    public Task<User?> GetByIdAsync(Guid id) =>
        _context.Users.FirstOrDefaultAsync(u => u.Id == id);

    public Task<User?> GetByIdWithRelationsAsync(Guid id) =>
        _context.Users
            .Include(u => u.Posts)
            .Include(u => u.Followers)
            .Include(u => u.Following)
            .FirstOrDefaultAsync(u => u.Id == id);

    public Task<User?> GetByUsernameAsync(string username) =>
        _context.Users.FirstOrDefaultAsync(u => u.NormalizedUserName == username.ToUpper());

    public Task<User?> GetByUsernameWithRelationsAsync(string username) =>
        _context.Users
            .Include(u => u.Posts)
            .Include(u => u.Followers)
            .Include(u => u.Following)
            .FirstOrDefaultAsync(u => u.UserName == username);

    public Task<List<User>> GetAllAsync() =>
        _context.Users.ToListAsync();

    public async Task AddAsync(User user) =>
        await _context.Users.AddAsync(user);

    public Task UpdateAsync(User user)
    {
        user.UpdatedAt = DateTime.UtcNow;
        _context.Users.Update(user);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync() =>
        _context.SaveChangesAsync();
}
