using Application.DTOs;

namespace Application.Interfaces;

public interface IPostService
{
    Task<PostDetailedViewDto?> GetPostByIdAsync(Guid id);
    Task<List<PostSimpleViewDto>> GetAllPostsAsync();
    Task<List<PostSimpleViewDto>> GetPostsByUsernameAsync(string username);
    Task<PostDetailedViewDto> CreatePostAsync(PostCreateDto postCreateDto, Guid userId, string? imageUrl);
    Task<PostDetailedViewDto?> UpdatePostAsync(Guid postId, PostUpdateDto postUpdateDto, Guid userId, string? imageUrl);
    Task<bool> DeletePostAsync(Guid postId, Guid userId);
}