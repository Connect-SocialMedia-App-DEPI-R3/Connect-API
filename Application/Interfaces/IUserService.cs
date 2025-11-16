using Application.DTOs;

namespace Application.Interfaces;

public interface IUserService
{
    Task<UserDto?> GetUserByIdAsync(string id);
    Task<List<UserDto>> GetAllUsersAsync();
    Task<UserDto> CreateUserAsync(string username, string email);
}
