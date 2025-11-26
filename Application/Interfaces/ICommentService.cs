using Application.DTOs;

namespace Application.Interfaces;

public interface ICommentService
{
    Task<CommentDto?> GetCommentByIdAsync(Guid id);
    Task<List<CommentDto>> GetAllCommentsAsync();
    Task<List<CommentDto>> GetAllCommentsByPostIdAsync(Guid postId);
    Task<CommentDto> CreateCommentAsync(CommentCreateDto createCommentDto, Guid postId, Guid userId);
    Task<CommentDto?> UpdateCommentAsync(Guid id, CommentUpdateDto updateDto, Guid userId);
    Task<bool> DeleteCommentAsync(Guid id, Guid userId);
}