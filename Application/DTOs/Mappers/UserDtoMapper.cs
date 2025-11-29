namespace Application.DTOs.Mappers;

using Domain.Entities;
using Application.DTOs;

public static class UserDtoMapper
{
    public static UserDto ToUserDto(this User user)
    {
        return new UserDto(
            user.Id,
            user.UserName ?? string.Empty,
            user.Email ?? string.Empty,
            user.AvatarUrl
        );
    }
}