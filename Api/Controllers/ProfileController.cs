using Api.Filters;
using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/profile")]
[Authorize]
public class ProfileController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IImageService _imageService;

    public ProfileController(IUserService userService, IImageService imageService)
    {
        _userService = userService;
        _imageService = imageService;
    }

    [HttpGet]
    [ExtractUserId]
    public async Task<ActionResult<UserProfileDto>> GetMyProfile()
    {
        var userId = (Guid)HttpContext.Items["UserId"]!;
        var user = await _userService.GetUserByIdAsync(userId);
        if (user == null)
            return NotFound();

        var profile = await _userService.GetUserProfileAsync(user.Username);
        return Ok(profile);
    }

    [HttpGet("{username}")]
    [AllowAnonymous]
    public async Task<ActionResult<UserProfileDto>> GetUserProfile(string username)
    {
        var profile = await _userService.GetUserProfileAsync(username);
        return Ok(profile);
    }

    [HttpPut]
    [ExtractUserId]
    public async Task<ActionResult<UserProfileDto>> UpdateProfile([FromBody] UserUpdateProfileDto updateDto)
    {
        var userId = (Guid)HttpContext.Items["UserId"]!;
        var profile = await _userService.UpdateUserProfileAsync(userId, updateDto);
        return Ok(profile);
    }

    [HttpPut("avatar")]
    [ExtractUserId]
    public async Task<ActionResult<object>> UpdateAvatar(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest(new { message = "No file uploaded" });

        var userId = (Guid)HttpContext.Items["UserId"]!;
        
        // Upload image using IImageService
        using var stream = file.OpenReadStream();
        var avatarUrl = await _imageService.UploadImageAsync(stream, file.FileName, file.ContentType);
        
        // Update user's avatar URL
        await _userService.UpdateUserAvatarAsync(userId, avatarUrl);
        
        return Ok(new { avatarUrl });
    }

    [HttpDelete]
    [ExtractUserId]
    public async Task<ActionResult> DeleteAccount()
    {
        var userId = (Guid)HttpContext.Items["UserId"]!;
        await _userService.DeleteUserAsync(userId);
        
        return Ok(new { message = "Account deleted successfully" });
    }
}
