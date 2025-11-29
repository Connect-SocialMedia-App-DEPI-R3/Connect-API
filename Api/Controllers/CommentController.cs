using Api.Filters;
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

        if (comment!.PostId != postId)
            return BadRequest(new { message = "Comment does not belong to the specified post" });

        return Ok(comment);
    }

    // POST: api/posts/{postId}/comments
    [Authorize]
    [HttpPost]
    [ExtractUserId]
    public async Task<ActionResult<CommentDto>> CreateComment(Guid postId, [FromBody] CommentCreateDto commentCreateDto)
    {
        var userId = (Guid)HttpContext.Items["UserId"]!;

        var comment = await _commentService.CreateCommentAsync(commentCreateDto, postId, userId);
        return CreatedAtAction(nameof(GetCommentById), new { postId, id = comment.Id }, comment);
    }

    // PUT: api/posts/{postId}/comments/{id}
    [Authorize]
    [HttpPut("{id}")]
    [ExtractUserId]
    public async Task<ActionResult<CommentDto>> UpdateComment(Guid postId, Guid id, [FromBody] CommentUpdateDto commentUpdateDto)
    {
        var userId = (Guid)HttpContext.Items["UserId"]!;

        var comment = await _commentService.UpdateCommentAsync(id, commentUpdateDto, userId);
        return Ok(comment);
    }

    // DELETE: api/posts/{postId}/comments/{id}
    [Authorize]
    [HttpDelete("{id}")]
    [ExtractUserId]
    public async Task<ActionResult> DeleteComment(Guid postId, Guid id)
    {
        var userId = (Guid)HttpContext.Items["UserId"]!;

        await _commentService.DeleteCommentAsync(id, userId);
        return NoContent();
    }
}
