using Application.DTOs;

namespace Application.Interfaces;

public interface IPostService
{
    Task<PostDetailedViewDto?> GetPostByIdAsync(Guid id);
    Task<List<PostSimpleViewDto>> GetAllPostsAsync();
    Task<PostDetailedViewDto> CreatePostAsync(PostCreateDto postCreateDto, Guid userId);
    Task<PostDetailedViewDto?> UpdatePostAsync(Guid postId, PostUpdateDto postUpdateDto, Guid userId);
    Task<bool> DeletePostAsync(Guid postId, Guid userId);
}