using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _repo;
    private readonly IImageService _imageService;

    public UserService(IUserRepository repo, IImageService imageService)
    {
        _repo = repo;
        _imageService = imageService;
    }

    public async Task<UserDto?> GetUserByIdAsync(Guid id)
    {
        var user = await _repo.GetByIdAsync(id);
        return user is null ? null : new(user.Id, user.UserName ?? string.Empty, user.Email ?? string.Empty, user.AvatarUrl);
    }

    public async Task<List<UserDto>> GetAllUsersAsync()
    {
        var users = await _repo.GetAllAsync();
        return users.Select(u => new UserDto(u.Id, u.UserName ?? string.Empty, u.Email ?? string.Empty, u.AvatarUrl)).ToList();
    }

    public async Task<UserDto> CreateUserAsync(string username, string email)
    {
        var entity = new User { Id = Guid.NewGuid(), UserName = username, Email = email };
        await _repo.AddAsync(entity);
        await _repo.SaveChangesAsync();
        return new(entity.Id, entity.UserName ?? string.Empty, entity.Email ?? string.Empty, entity.AvatarUrl);
    }

    public async Task<UserProfileDto> GetUserProfileAsync(Guid userId)
    {
        var user = await _repo.GetByIdAsync(userId);
        if (user is null)
            throw new KeyNotFoundException($"User with ID {userId} not found.");

        return new UserProfileDto
        {
            Id = user.Id,
            Username = user.UserName ?? string.Empty,
            Email = user.Email ?? string.Empty,
            Bio = user.Bio,
            AvatarUrl = user.AvatarUrl,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };
    }

    public async Task<UserProfileDto> UpdateUserProfileAsync(Guid userId, UserUpdateProfileDto updateDto)
    {
        var user = await _repo.GetByIdAsync(userId);
        if (user is null)
            throw new KeyNotFoundException($"User with ID {userId} not found.");

        // Check if username is taken by another user
        // if (!string.IsNullOrWhiteSpace(updateDto.Username) && updateDto.Username != user.UserName)
        // {
        //     var existingUser = await _repo.GetByUsernameAsync(updateDto.Username);
        //     if (existingUser is not null)
        //         throw new InvalidOperationException("Username is already taken.");
            
        //     user.UserName = updateDto.Username;
        // }

        if (updateDto.Bio is not null)
            user.Bio = updateDto.Bio;

        await _repo.UpdateAsync(user);
        await _repo.SaveChangesAsync();

        return new UserProfileDto
        {
            Id = user.Id,
            Username = user.UserName ?? string.Empty,
            Email = user.Email ?? string.Empty,
            Bio = user.Bio,
            AvatarUrl = user.AvatarUrl,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };
    }

    public async Task<string> UpdateUserAvatarAsync(Guid userId, string avatarUrl)
    {
        var user = await _repo.GetByIdAsync(userId);
        if (user is null)
            throw new KeyNotFoundException($"User with ID {userId} not found.");

        // Delete old avatar if exists
        if (!string.IsNullOrEmpty(user.AvatarUrl))
        {
            await _imageService.DeleteImageAsync(user.AvatarUrl);
        }

        // Set new avatar URL
        user.AvatarUrl = avatarUrl;

        await _repo.UpdateAsync(user);
        await _repo.SaveChangesAsync();

        return avatarUrl;
    }

    public async Task<bool> SoftDeleteUserAsync(Guid userId)
    {
        var user = await _repo.GetByIdAsync(userId);
        if (user is null)
            throw new KeyNotFoundException($"User with ID {userId} not found.");

        user.IsDeleted = true;
        user.DeletedAt = DateTime.UtcNow;

        await _repo.UpdateAsync(user);
        await _repo.SaveChangesAsync();

        return true;
    }
}
