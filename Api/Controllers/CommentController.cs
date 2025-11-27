using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Api.Controllers;

[ApiController]
[Route("api/posts/{postId}/comments")]
public class CommentController : ControllerBase
{
    private readonly ICommentService _commentService;

    public CommentController(ICommentService commentService)
    {
        _commentService = commentService;
    }

    // GET: api/posts/{postId}/comments
    [HttpGet]
    public async Task<ActionResult<List<CommentDto>>> GetCommentsByPostId(Guid postId)
    {
        var comments = await _commentService.GetAllCommentsByPostIdAsync(postId);
        return Ok(comments);
    }

    // GET: api/posts/{postId}/comments/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<CommentDto>> GetCommentById(Guid postId, Guid id)
    {
        var comment = await _commentService.GetCommentByIdAsync(id);

        if (comment is null)
            return NotFound(new { message = "Comment not found" });

        if (comment.PostId != postId)
            return BadRequest(new { message = "Comment does not belong to the specified post" });

        return Ok(comment);
    }

    // POST: api/posts/{postId}/comments
    [Authorize]
    [HttpPost]
    public async Task<ActionResult<CommentDto>> CreateComment(Guid postId, [FromBody] CommentCreateDto commentCreateDto)
    {
        var userId = GetUserIdFromClaims();

        if (userId == Guid.Empty)
            return Unauthorized(new { message = "Invalid user credentials" });

        try
        {
            var comment = await _commentService.CreateCommentAsync(commentCreateDto, postId, userId);
            return CreatedAtAction(nameof(GetCommentById), new { postId, id = comment.Id }, comment);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    // PUT: api/posts/{postId}/comments/{id}
    [Authorize]
    [HttpPut("{id}")]
    public async Task<ActionResult<CommentDto>> UpdateComment(Guid postId, Guid id, [FromBody] CommentUpdateDto commentUpdateDto)
    {
        var userId = GetUserIdFromClaims();

        if (userId == Guid.Empty)
            return Unauthorized(new { message = "Invalid user credentials" });

        try
        {
            var comment = await _commentService.UpdateCommentAsync(id, commentUpdateDto, userId);

            if (comment is null)
                return NotFound(new { message = "Comment not found" });

            return Ok(comment);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    // DELETE: api/posts/{postId}/comments/{id}
    [Authorize]
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteComment(Guid postId, Guid id)
    {
        var userId = GetUserIdFromClaims();

        if (userId == Guid.Empty)
            return Unauthorized(new { message = "Invalid user credentials" });

        try
        {
            var result = await _commentService.DeleteCommentAsync(id, userId);

            if (!result)
                return NotFound(new { message = "Comment not found" });

            return NoContent();
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    private Guid GetUserIdFromClaims()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
    }
}
