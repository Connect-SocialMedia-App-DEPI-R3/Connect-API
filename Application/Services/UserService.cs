using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _repo;

    public UserService(IUserRepository repo) => _repo = repo;

    public async Task<UserDto?> GetUserByIdAsync(string id)
    {
        var user = await _repo.GetByIdAsync(id);
        return user is null ? null : new(user.Id, user.UserName ?? string.Empty, user.Email ?? string.Empty);
    }

    public async Task<List<UserDto>> GetAllUsersAsync()
    {
        var users = await _repo.GetAllAsync();
        return users.Select(u => new UserDto(u.Id, u.UserName ?? string.Empty, u.Email ?? string.Empty)).ToList();
    }

    public async Task<UserDto> CreateUserAsync(string username, string email)
    {
        var entity = new User { Id = Guid.NewGuid().ToString(), UserName = username, Email = email };
        await _repo.AddAsync(entity);
        await _repo.SaveChangesAsync();
        return new(entity.Id, entity.UserName ?? string.Empty, entity.Email ?? string.Empty);
    }
}
