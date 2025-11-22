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
}