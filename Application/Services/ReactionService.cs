using Application.DTOs;
using Application.DTOs.Mappers;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services;

public class ReactionService : IReactionService
{
    private readonly IReactionRepository _reactionRepo;
    private readonly IPostRepository _postRepo;

    public ReactionService(IReactionRepository reactionRepo, IPostRepository postRepo)
    {
        _reactionRepo = reactionRepo;
        _postRepo = postRepo;
    }

    public async Task<bool> ToggleReactionAsync(Guid userId, Guid postId)
    {
        // Check if post exists
        var post = await _postRepo.GetByIdAsync(postId);
        if (post is null)
            throw new KeyNotFoundException($"Post with ID {postId} not found");

        // Check if user already reacted
        var existingReaction = await _reactionRepo.GetReactionAsync(userId, postId);
        
        if (existingReaction is not null)
        {
            // Already reacted - remove reaction
            await _reactionRepo.DeleteAsync(existingReaction);
            await _reactionRepo.SaveChangesAsync();
            return false; // Unreacted
        }
        else
        {
            // Not reacted - add reaction
            var reaction = new Reaction
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                PostId = postId,
                CreatedAt = DateTime.UtcNow
            };
            await _reactionRepo.AddAsync(reaction);
            await _reactionRepo.SaveChangesAsync();
            return true; // Now reacted
        }
    }

    public async Task<List<ReactionDto>> GetPostReactionsAsync(Guid postId)
    {
        var reactions = await _reactionRepo.GetPostReactionsAsync(postId);
        return reactions.Select(r => r.ToReactionDto()).ToList();
    }

    public Task<bool> HasUserReactedAsync(Guid userId, Guid postId) =>
        _reactionRepo.HasUserReactedAsync(userId, postId);
}
