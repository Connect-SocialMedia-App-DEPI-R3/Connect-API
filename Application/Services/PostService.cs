using Application.DTOs;
using Application.DTOs.Mappers;
using Application.Interfaces;
using Domain.Interfaces;
using Domain.Entities;

namespace Application.Services;

public class PostService : IPostService
{
    private readonly IPostRepository _postRepository;
    private readonly IImageService _imageService;

    public PostService(IPostRepository postRepository, IImageService imageService)
    {
        _postRepository = postRepository;
        _imageService = imageService;
    }

    public async Task<PostDetailedViewDto?> GetPostByIdAsync(Guid id)
    {
        var post = await _postRepository.GetByIdAsync(id);
        return post?.ToPostDetailedViewDto();
    }

    public async Task<List<PostSimpleViewDto>> GetAllPostsAsync()
    {
        var posts = await _postRepository.GetAllAsync();
        return posts.Select(post => post.ToPostSimpleViewDto()).ToList();
    }

    public async Task<List<PostSimpleViewDto>> GetPostsByUsernameAsync(string username)
    {
        var posts = await _postRepository.GetByUsernameAsync(username);
        return posts.Select(post => post.ToPostSimpleViewDto()).ToList();
    }

    public async Task<PostDetailedViewDto> CreatePostAsync(PostCreateDto postCreateDto, Guid userId, string? imageUrl)
    {
        var post = postCreateDto.ToEntity(userId);
        post.ImageUrl = imageUrl;

        await _postRepository.AddAsync(post);
        await _postRepository.SaveChangesAsync();
        
        var createdPost = await _postRepository.GetByIdAsync(post.Id);
        return createdPost!.ToPostDetailedViewDto();
    }

    public async Task<PostDetailedViewDto?> UpdatePostAsync(Guid postId, PostUpdateDto postUpdateDto, Guid userId, string? imageUrl)
    {
        var post = await _postRepository.GetByIdAsync(postId);
        
        if (post is null)
            throw new KeyNotFoundException($"Post with ID {postId} not found.");

        // Check ownership
        if (post.UserId != userId)
            throw new UnauthorizedAccessException("You are not authorized to update this post.");

        // Update title and content using the UpdateEntity method
        postUpdateDto.UpdateEntity(post);

        // Handle image updates AFTER updating other fields
        if (postUpdateDto.RemoveImage)
        {
            // Delete existing image
            if (!string.IsNullOrEmpty(post.ImageUrl))
            {
                await _imageService.DeleteImageAsync(post.ImageUrl);
            }
            post.ImageUrl = null;
        }
        else if (imageUrl != null)
        {
            // Replace with new image
            if (!string.IsNullOrEmpty(post.ImageUrl))
            {
                await _imageService.DeleteImageAsync(post.ImageUrl);
            }
            post.ImageUrl = imageUrl;
        }
        // If neither flag nor new image: keep existing image unchanged

        await _postRepository.UpdateAsync(post);
        await _postRepository.SaveChangesAsync();

        var updatedPost = await _postRepository.GetByIdAsync(postId);
        return updatedPost?.ToPostDetailedViewDto();
    }

    public async Task<bool> DeletePostAsync(Guid postId, Guid userId)
    {
        var post = await _postRepository.GetByIdAsync(postId);
        
        if (post is null)
            throw new KeyNotFoundException($"Post with ID {postId} not found.");

        // Check ownership
        if (post.UserId != userId)
            throw new UnauthorizedAccessException("You are not authorized to delete this post.");

        // Delete associated image if exists
        if (!string.IsNullOrEmpty(post.ImageUrl))
        {
            await _imageService.DeleteImageAsync(post.ImageUrl);
        }

        await _postRepository.DeleteAsync(post);
        await _postRepository.SaveChangesAsync();
        
        return true;
    }
}