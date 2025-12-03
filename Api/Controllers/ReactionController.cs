using Api.Filters;
using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/reactions")]
[Authorize]
public class ReactionController : ControllerBase
{
    private readonly IReactionService _reactionService;

    public ReactionController(IReactionService reactionService)
    {
        _reactionService = reactionService;
    }

    [HttpPost("{postId}")]
    [ExtractUserId]
    public async Task<ActionResult> ToggleReaction(Guid postId)
    {
        var userId = (Guid)HttpContext.Items["UserId"]!;
        var isNowReacted = await _reactionService.ToggleReactionAsync(userId, postId);
        
        return Ok(new { 
            hasReacted = isNowReacted,
            message = isNowReacted ? "Reacted to post" : "Removed reaction"
        });
    }

    [HttpGet("{postId}")]
    [AllowAnonymous]
    public async Task<ActionResult<List<ReactionDto>>> GetPostReactions(Guid postId)
    {
        var reactions = await _reactionService.GetPostReactionsAsync(postId);
        return Ok(reactions);
    }

    [HttpGet("{postId}/has-reacted")]
    [ExtractUserId]
    public async Task<ActionResult<object>> HasUserReacted(Guid postId)
    {
        var userId = (Guid)HttpContext.Items["UserId"]!;
        var hasReacted = await _reactionService.HasUserReactedAsync(userId, postId);
        return Ok(new { hasReacted });
    }
}
