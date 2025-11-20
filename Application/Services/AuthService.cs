using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Application.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly ITokenService _tokenService;

    public AuthService(UserManager<User> userManager, ITokenService tokenService)
    {
        _userManager = userManager;
        _tokenService = tokenService;
    }
    public async Task<AuthResponseDto?> LoginAsync(UserLoginDto loginDto)
    {
        var user = await _userManager.FindByEmailAsync(loginDto.Email);
        if (user is null)
            return null;
        
        var check = await _userManager.CheckPasswordAsync(user, loginDto.Password);
        if (!check) 
            return null;
        
        var roles = await _userManager.GetRolesAsync(user);
        var token = _tokenService.GenerateAccessToken(user, roles);
        return new AuthResponseDto(
            Token: token,
            Email: user.Email!
        );

    }

    public async Task<AuthResponseDto?> RegisterAsync(UserRegisterDto registerDto)
    {
        var user = new User
        {
            Email = registerDto.Email,
            UserName = registerDto.Username
        };

        var result = await _userManager.CreateAsync(user, registerDto.Password);

        if (!result.Succeeded)
            throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

        var roles = await _userManager.GetRolesAsync(user);
        var token = _tokenService.GenerateAccessToken(user, roles);

        return new AuthResponseDto(
            Token: token,
            Email: user.Email!
        );
    }

    public Task LogoutAsync()
    {
        // JWT tokens are stateless
        // Logout is handled client-side by removing the token
        return Task.CompletedTask;
    }
}