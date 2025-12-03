namespace Application.DTOs;

public record ReactionDto(
    Guid Id,
    UserDto User,
    Guid PostId,
    DateTime CreatedAt
);
