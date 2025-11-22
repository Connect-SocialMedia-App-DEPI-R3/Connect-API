using Application.DTOs;
using Domain.Entities;

namespace Application.DTOs.Mappers;

public static class PostDtoMapper
{
    public static PostSimpleViewDto ToPostSimpleViewDto(this Post post)
    {
        return new PostSimpleViewDto
        {
            Id = post.Id,
            Title = post.Title,
            Content = post.Content,
            ImageUrl = post.ImageUrl,
            CreatedAt = post.CreatedAt,
            AuthorId = post.User.Id,
            AuthorUsername = post.User.UserName ?? string.Empty,
            LikeCount = post.Reactions?.Count ?? 0,
            CommentCount = post.Comments?.Count ?? 0
        };
    }

    public static PostDetailedViewDto ToPostDetailedViewDto(this Post post)
    {
        return new PostDetailedViewDto
        {
            Id = post.Id,
            Title = post.Title,
            Content = post.Content,
            ImageUrl = post.ImageUrl,
            CreatedAt = post.CreatedAt,
            Author = post.User.ToUserDto(),
            Comments = post.Comments?.Select(c => c.ToCommentDto()).ToList() ?? [],
            LikeCount = post.Reactions?.Count ?? 0
        };
    }

    public static Post ToEntity(this PostCreateDto postCreateDto, Guid userId)
    {
        return new Post
        {
            Id = Guid.NewGuid(),
            Title = postCreateDto.Title,
            Content = postCreateDto.Content,
            ImageUrl = postCreateDto.ImageUrl,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            UserId = userId
        };
    }

    public static void UpdateEntity(this PostUpdateDto postUpdateDto, Post post)
    {
        post.Title = postUpdateDto.Title;
        post.Content = postUpdateDto.Content;
        post.ImageUrl = postUpdateDto.ImageUrl;
        post.UpdatedAt = DateTime.UtcNow;
    }

}