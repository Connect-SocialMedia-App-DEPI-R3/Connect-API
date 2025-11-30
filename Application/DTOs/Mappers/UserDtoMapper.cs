namespace Application.DTOs.Mappers;

using Domain.Entities;
using Application.DTOs;

public static class UserDtoMapper
{
    public static UserDto ToUserDto(this User user)
    {
        return new UserDto(
            user.Id,
            user.FullName ?? string.Empty,
            user.UserName ?? string.Empty,
            user.AvatarUrl
        );
    }

    public static UserProfileDto ToUserProfileDto(this User user)
    {
        return new UserProfileDto
        {
            Id = user.Id,
            FullName = user.FullName ?? string.Empty,
            Username = user.UserName ?? string.Empty,
            Email = user.Email ?? string.Empty,
            Bio = user.Bio,
            FollowerCount = user.Followers?.Count ?? 0,
            FollowingCount = user.Following?.Count ?? 0,
            AvatarUrl = user.AvatarUrl,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };
    }

    public static User ToEntity(this UserRegisterDto registerDto)
    {
        return new User
        {
            FullName = registerDto.FullName ?? string.Empty,
            UserName = registerDto.Username,
            Email = registerDto.Email
        };
    }
}