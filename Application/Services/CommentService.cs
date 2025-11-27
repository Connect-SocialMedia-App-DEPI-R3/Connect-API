using Application.DTOs;
using Application.DTOs.Mappers;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services;

public class CommentService : ICommentService
{
    private readonly ICommentRepository _commentRepository;
    private readonly IPostRepository _postRepository;

    public CommentService(ICommentRepository commentRepository, IPostRepository postRepository)
    {
        _commentRepository = commentRepository;
        _postRepository = postRepository;
    }

    public async Task<CommentDto?> GetCommentByIdAsync(Guid id)
    {
        var comment = await _commentRepository.GetByIdAsync(id);
        return comment?.ToCommentDto();
    }

    public async Task<List<CommentDto>> GetAllCommentsAsync()
    {
        var comments = await _commentRepository.GetAllAsync();
        return comments.Select(c => c.ToCommentDto()).ToList();
    }

    public async Task<List<CommentDto>> GetAllCommentsByPostIdAsync(Guid postId)
    {
        var comments = await _commentRepository.GetAllByPostIdAsync(postId);
        return comments.Select(c => c.ToCommentDto()).ToList();
    }

    public async Task<CommentDto> CreateCommentAsync(CommentCreateDto createCommentDto, Guid postId, Guid userId)
    {
        // Validate post exists
        var post = await _postRepository.GetByIdAsync(postId);
        if (post is null)
            throw new KeyNotFoundException($"Post with ID {postId} not found.");

        var comment = createCommentDto.ToEntity(postId, userId);

        await _commentRepository.AddAsync(comment);
        await _commentRepository.SaveChangesAsync();

        var createdComment = await _commentRepository.GetByIdAsync(comment.Id);
        return createdComment!.ToCommentDto();
    }

    public async Task<CommentDto?> UpdateCommentAsync(Guid id, CommentUpdateDto updateDto, Guid userId)
    {
        var comment = await _commentRepository.GetByIdAsync(id);

        if (comment is null)
            return null;

        // Check ownership
        if (comment.UserId != userId)
            throw new UnauthorizedAccessException("You are not authorized to update this comment.");

        updateDto.UpdateEntity(comment);

        await _commentRepository.UpdateAsync(comment);
        await _commentRepository.SaveChangesAsync();

        var updatedComment = await _commentRepository.GetByIdAsync(id);
        return updatedComment?.ToCommentDto();
    }

    public async Task<bool> DeleteCommentAsync(Guid id, Guid userId)
    {
        var comment = await _commentRepository.GetByIdAsync(id);

        if (comment is null)
            return false;

        // Check ownership
        if (comment.UserId != userId)
            throw new UnauthorizedAccessException("You are not authorized to delete this comment.");

        await _commentRepository.DeleteAsync(comment);
        await _commentRepository.SaveChangesAsync();

        return true;
    }
}