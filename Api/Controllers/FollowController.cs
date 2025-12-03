using Api.Filters;
using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/follow")]
[Authorize]
public class FollowController : ControllerBase
{
    private readonly IUserService _userService;

    public FollowController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("{username}")]
    [ExtractUserId]
    public async Task<ActionResult> ToggleFollow(string username)
    {
        var userId = (Guid)HttpContext.Items["UserId"]!;
        var isNowFollowing = await _userService.ToggleFollowAsync(userId, username);
        
        return Ok(new { 
            isFollowing = isNowFollowing,
            message = isNowFollowing ? "Successfully followed user" : "Successfully unfollowed user"
        });
    }

    [HttpGet("{username}/followers")]
    public async Task<ActionResult<List<UserDto>>> GetFollowers(string username)
    {
        var followers = await _userService.GetFollowersAsync(username);
        return Ok(followers);
    }

    [HttpGet("{username}/following")]
    public async Task<ActionResult<List<UserDto>>> GetFollowing(string username)
    {
        var following = await _userService.GetFollowingAsync(username);
        return Ok(following);
    }

    [HttpGet("{username}/is-following")]
    [ExtractUserId]
    public async Task<ActionResult<object>> IsFollowing(string username)
    {
        var userId = (Guid)HttpContext.Items["UserId"]!;
        var isFollowing = await _userService.IsFollowingAsync(userId, username);
        return Ok(new { isFollowing });
    }
}
