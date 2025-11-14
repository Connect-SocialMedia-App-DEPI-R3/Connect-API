using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _repo;

    public UserService(IUserRepository repo) => _repo = repo;

    public async Task<UserDto?> GetUserByIdAsync(Guid id)
    {
        var user = await _repo.GetByIdAsync(id);
        return user is null ? null : new(user.Id, user.Username, user.Email);
    }

    public async Task<List<UserDto>> GetAllUsersAsync()
    {
        var users = await _repo.GetAllAsync();
        return users.Select(u => new UserDto(u.Id, u.Username, u.Email)).ToList();
    }

    public async Task<UserDto> CreateUserAsync(string username, string email)
    {
        var entity = new User { Id = Guid.NewGuid(), Username = username, Email = email };
        await _repo.AddAsync(entity);
        await _repo.SaveChangesAsync();
        return new(entity.Id, entity.Username, entity.Email);
    }
}
