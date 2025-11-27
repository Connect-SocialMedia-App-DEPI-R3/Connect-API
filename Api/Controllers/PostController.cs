using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Api.Controllers;

[ApiController]
[Route("api/posts")]
public class PostController : ControllerBase
{
    private readonly IPostService _postService;

    public PostController(IPostService postService)
    {
        _postService = postService;
    }

    // GET: api/posts
    [HttpGet]
    public async Task<ActionResult<List<PostSimpleViewDto>>> GetAllPosts()
    {
        var posts = await _postService.GetAllPostsAsync();
        return Ok(posts);
    }

    // GET: api/posts/{id}
    [HttpGet("{id:Guid}")]
    public async Task<ActionResult<PostDetailedViewDto>> GetPostById(Guid id)
    {
        var post = await _postService.GetPostByIdAsync(id);
        
        if (post is null)
            return NotFound(new { message = "Post not found" });

        return Ok(post);
    }

    // POST: api/posts
    [Authorize]
    [HttpPost]
    public async Task<ActionResult<PostDetailedViewDto>> CreatePost([FromBody] PostCreateDto postCreateDto)
    {
        var userId = GetUserIdFromClaims();
        
        if (userId == Guid.Empty)
            return Unauthorized(new { message = "Invalid user credentials" });

        var post = await _postService.CreatePostAsync(postCreateDto, userId);
        return CreatedAtAction(nameof(GetPostById), new { id = post.Id }, post);
    }

    // PUT: api/posts/{id}
    [Authorize]
    [HttpPut("{id}")]
    public async Task<ActionResult<PostDetailedViewDto>> UpdatePost(Guid id, [FromBody] PostUpdateDto postUpdateDto)
    {
        var userId = GetUserIdFromClaims();
        
        if (userId == Guid.Empty)
            return Unauthorized(new { message = "Invalid user credentials" });

        try
        {
            var post = await _postService.UpdatePostAsync(id, postUpdateDto, userId);
            
            if (post is null)
                return NotFound(new { message = "Post not found" });

            return Ok(post);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    // DELETE: api/posts/{id}
    [Authorize]
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeletePost(Guid id)
    {
        var userId = GetUserIdFromClaims();
        
        if (userId == Guid.Empty)
            return Unauthorized(new { message = "Invalid user credentials" });

        try
        {
            var result = await _postService.DeletePostAsync(id, userId);
            
            if (!result)
                return NotFound(new { message = "Post not found" });

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
