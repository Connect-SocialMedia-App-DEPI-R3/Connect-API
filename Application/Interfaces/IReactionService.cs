using Application.DTOs;

namespace Application.Interfaces;

public interface IReactionService
{
    Task<bool> ToggleReactionAsync(Guid userId, Guid postId);
    Task<List<ReactionDto>> GetPostReactionsAsync(Guid postId);
    Task<bool> HasUserReactedAsync(Guid userId, Guid postId);
}
