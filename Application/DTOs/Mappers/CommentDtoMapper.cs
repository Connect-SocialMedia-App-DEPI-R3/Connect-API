namespace Application.DTOs.Mappers;

using Domain.Entities;
using Application.DTOs;

public static class CommentDtoMapper
{
    public static CommentDto ToCommentDto(this Comment comment)
    {
        return new CommentDto
        {
            Id = comment.Id,
            Content = comment.Content,
            CreatedAt = comment.CreatedAt,
            Author = comment.User.ToUserDto()
        };
    }

    public static Comment ToEntity(this CommentCreateDto createDto, Guid postId, Guid userId)
    {
        return new Comment
        {
            Id = Guid.NewGuid(),
            Content = createDto.Content,
            PostId = postId,
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };
    }

    public static void UpdateEntity(this CommentUpdateDto updateDto, Comment comment)
    {
        comment.Content = updateDto.Content;
    }
}