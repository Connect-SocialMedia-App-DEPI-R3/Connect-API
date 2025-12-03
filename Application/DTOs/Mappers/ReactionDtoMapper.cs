using Domain.Entities;

namespace Application.DTOs.Mappers;

public static class ReactionDtoMapper
{
    public static ReactionDto ToReactionDto(this Reaction reaction)
    {
        return new ReactionDto(
            reaction.Id,
            reaction.User.ToUserDto(),
            reaction.PostId,
            reaction.CreatedAt
        );
    }
}
