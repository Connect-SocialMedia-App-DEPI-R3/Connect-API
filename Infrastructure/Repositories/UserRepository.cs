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
        _context.Users.FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted);

    public Task<User?> GetByUsernameAsync(string username) =>
        _context.Users.FirstOrDefaultAsync(u => u.UserName == username && !u.IsDeleted);

    public Task<List<User>> GetAllAsync() =>
        _context.Users.Where(u => !u.IsDeleted).ToListAsync();

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
