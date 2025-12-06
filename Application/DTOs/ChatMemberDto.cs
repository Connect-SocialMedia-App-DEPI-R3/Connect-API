using Domain.Entities;

namespace Application.DTOs;

public record ChatMemberDto(
    UserDto User,
    ChatRole Role,
    DateTime JoinedAt
);
