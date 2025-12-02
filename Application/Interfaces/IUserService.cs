using Application.DTOs;

namespace Application.Interfaces;

public interface IUserService
{
    Task<UserDto?> GetUserByIdAsync(Guid id);
    Task<List<UserDto>> GetAllUsersAsync();
    Task<UserDto> CreateUserAsync(string username, string email);
    Task<UserProfileDto> GetUserProfileAsync(Guid userId);
    Task<UserProfileDto> UpdateUserProfileAsync(Guid userId, UserUpdateProfileDto updateDto);
    Task<string> UpdateUserAvatarAsync(Guid userId, string avatarUrl);
    Task<bool> DeleteUserAsync(Guid userId);
}
