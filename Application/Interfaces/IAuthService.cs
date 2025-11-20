using Application.DTOs;

namespace Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto?> LoginAsync(UserLoginDto loginDto);
    Task<AuthResponseDto?> RegisterAsync(UserRegisterDto registerDto);
    Task LogoutAsync();
}