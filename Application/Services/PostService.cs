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

    public async Task<PostDetailedViewDto> CreatePostAsync(PostCreateDto postCreateDto, Guid userId)
    {
        var post = postCreateDto.ToEntity(userId);

        await _postRepository.AddAsync(post);
        await _postRepository.SaveChangesAsync();
        
        var createdPost = await _postRepository.GetByIdAsync(post.Id);
        return createdPost!.ToPostDetailedViewDto();
    }

    public async Task<PostDetailedViewDto?> UpdatePostAsync(Guid postId, PostUpdateDto postUpdateDto, Guid userId)
    {
        var post = await _postRepository.GetByIdAsync(postId);
        
        if (post is null)
            return null;

        // Check ownership
        if (post.UserId != userId)
            throw new UnauthorizedAccessException("You are not authorized to update this post.");

        // delete old image if it's being replaced
        if (!string.IsNullOrEmpty(post.ImageUrl) && post.ImageUrl != postUpdateDto.ImageUrl)
        {
            await _imageService.DeleteImageAsync(post.ImageUrl);
        }
        
        postUpdateDto.UpdateEntity(post);

        await _postRepository.UpdateAsync(post);
        await _postRepository.SaveChangesAsync();

        var updatedPost = await _postRepository.GetByIdAsync(postId);
        return updatedPost?.ToPostDetailedViewDto();
    }

    public async Task<bool> DeletePostAsync(Guid postId, Guid userId)
    {
        var post = await _postRepository.GetByIdAsync(postId);
        
        if (post is null)
            return false;

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