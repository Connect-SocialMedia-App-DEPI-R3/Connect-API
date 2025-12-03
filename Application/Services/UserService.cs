using System.Runtime.CompilerServices;
using Application.DTOs;
using Application.DTOs.Mappers;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;

namespace Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _repo;
    private readonly IFollowRepository _followRepo;
    private readonly IImageService _imageService;
    private readonly UserManager<User> _userManager;

    public UserService(IUserRepository repo, IFollowRepository followRepo, IImageService imageService, UserManager<User> userManager)
    {
        _repo = repo;
        _followRepo = followRepo;
        _imageService = imageService;
        _userManager = userManager;
    }

    public async Task<UserDto?> GetUserByIdAsync(Guid id)
    {
        var user = await _repo.GetByIdAsync(id);
        return user?.ToUserDto();
    }

    public async Task<List<UserDto>> GetAllUsersAsync()
    {
        var users = await _repo.GetAllAsync();
        return users.Select(u => u.ToUserDto()).ToList();
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
        var user = await _repo.GetByIdWithRelationsAsync(userId);
        if (user is null)
            throw new KeyNotFoundException($"User with ID {userId} not found.");

        return user.ToUserProfileDto();
    }

    public async Task<UserProfileDto> UpdateUserProfileAsync(Guid userId, UserUpdateProfileDto updateDto)
    {
        var user = await _repo.GetByIdAsync(userId);
        if (user is null)
            throw new KeyNotFoundException($"User with ID {userId} not found.");

        // change username
        if (!string.IsNullOrWhiteSpace(updateDto.Username) && updateDto.Username != user.UserName)
        {
            // check if username is taken by another user
            var existingUser = await _repo.GetByUsernameAsync(updateDto.Username);
            if (existingUser is not null)
                throw new InvalidOperationException("Username is already taken.");

            await _userManager.SetUserNameAsync(user, updateDto.Username);
        }

        // change full name
        if (!string.IsNullOrWhiteSpace(updateDto.FullName))
            user.FullName = updateDto.FullName;

        // change bio
        if (updateDto.Bio is not null)
            user.Bio = updateDto.Bio;

        await _repo.UpdateAsync(user);
        await _repo.SaveChangesAsync();

        return user.ToUserProfileDto();
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

    public async Task<bool> DeleteUserAsync(Guid userId)
    {
        var user = await _repo.GetByIdAsync(userId);
        if (user is null)
            throw new KeyNotFoundException($"User with ID {userId} not found.");

        // delete user avatar if exists
        if (!string.IsNullOrEmpty(user.AvatarUrl))
        {
            await _imageService.DeleteImageAsync(user.AvatarUrl);
        }

        // Use UserManager to safely delete Identity user (handles AspNetUserRoles, AspNetUserClaims, etc.)
        var result = await _userManager.DeleteAsync(user);
        if (!result.Succeeded)
        {
            throw new InvalidOperationException($"Failed to delete user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }

        return true;
    }

    public async Task<bool> ToggleFollowAsync(Guid followerId, string targetUsername)
    {
        // Make sure target user exists
        var targetUser = await _repo.GetByUsernameAsync(targetUsername);
        if (targetUser is null)
            throw new KeyNotFoundException($"User '{targetUsername}' not found");

        // Can't follow yourself
        if (followerId == targetUser.Id)
            throw new InvalidOperationException("You cannot follow yourself");

        // Check if already following
        var existingFollow = await _followRepo.GetFollowAsync(followerId, targetUser.Id);
        
        if (existingFollow is not null)
        {
            // Already following - unfollow
            await _followRepo.DeleteAsync(existingFollow);
            await _followRepo.SaveChangesAsync();
            return false; // Unfollowed
        }
        else
        {
            // Not following - follow
            var follow = new Follow
            {
                Id = Guid.NewGuid(),
                FollowerId = followerId,
                FollowingId = targetUser.Id
            };
            await _followRepo.AddAsync(follow);
            await _followRepo.SaveChangesAsync();
            return true; // Now following
        }
    }

    public async Task<List<UserDto>> GetFollowersAsync(string username)
    {
        var user = await _repo.GetByUsernameAsync(username);
        if (user is null)
            throw new KeyNotFoundException($"User '{username}' not found");

        var followers = await _followRepo.GetFollowersAsync(user.Id);
        return followers.Select(f => f.Follower.ToUserDto()).ToList();
    }

    public async Task<List<UserDto>> GetFollowingAsync(string username)
    {
        var user = await _repo.GetByUsernameAsync(username);
        if (user is null)
            throw new KeyNotFoundException($"User '{username}' not found");

        var following = await _followRepo.GetFollowingAsync(user.Id);
        return following.Select(f => f.Following.ToUserDto()).ToList();
    }

    public async Task<bool> IsFollowingAsync(Guid followerId, string targetUsername)
    {
        var targetUser = await _repo.GetByUsernameAsync(targetUsername);
        if (targetUser is null)
            return false;

        return await _followRepo.IsFollowingAsync(followerId, targetUser.Id);
    }
}
