using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Api.Filters;

namespace Api.Controllers;

[ApiController]
[Route("api/posts")]
public class PostController : ControllerBase
{
    private readonly IPostService _postService;
    private readonly IImageService _imageService;

    public PostController(IPostService postService, IImageService imageService)
    {
        _postService = postService;
        _imageService = imageService;
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
    [ExtractUserId]
    public async Task<ActionResult<PostDetailedViewDto>> CreatePost([FromForm] PostCreateDto postCreateDto, IFormFile? file)
    {
        var userId = (Guid)HttpContext.Items["UserId"]!;

        string? imageUrl = null;
        if (file != null && file.Length > 0)
        {
            using var stream = file.OpenReadStream();
            imageUrl = await _imageService.UploadImageAsync(stream, file.FileName, file.ContentType);
        }

        var post = await _postService.CreatePostAsync(postCreateDto, userId, imageUrl);
        return CreatedAtAction(nameof(GetPostById), new { id = post.Id }, post);
    }

    // PUT: api/posts/{id}
    [Authorize]
    [HttpPut("{id}")]
    [ExtractUserId]
    public async Task<ActionResult<PostDetailedViewDto>> UpdatePost(Guid id, [FromForm] PostUpdateDto postUpdateDto, IFormFile? file)
    {
        var userId = (Guid)HttpContext.Items["UserId"]!;

        string? imageUrl = null;
        if (file != null && file.Length > 0)
        {
            using var stream = file.OpenReadStream();
            imageUrl = await _imageService.UploadImageAsync(stream, file.FileName, file.ContentType);
        }
        // If file is null, imageUrl stays null and service won't update the image

        var post = await _postService.UpdatePostAsync(id, postUpdateDto, userId, imageUrl);

        if (post is null)
            return NotFound(new { message = "Post not found" });

        return Ok(post);
    }
    
    // GET: api/posts/u/{username}
    [HttpGet("u/{username}")]
    public async Task<ActionResult<List<PostSimpleViewDto>>> GetPostsByUsername(string username)
    {
        var posts = await _postService.GetPostsByUsernameAsync(username);
        return Ok(posts);
    }

    // DELETE: api/posts/{id}
    [Authorize]
    [HttpDelete("{id}")]
    [ExtractUserId]
    public async Task<ActionResult> DeletePost(Guid id)
    {
        var userId = (Guid)HttpContext.Items["UserId"]!;

        var result = await _postService.DeletePostAsync(id, userId);
        
        if (!result)
            return NotFound(new { message = "Post not found" });

        return NoContent();
    }
}
