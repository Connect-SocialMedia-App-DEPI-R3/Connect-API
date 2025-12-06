using Domain.Entities;

namespace Application.DTOs;

public record ChatDto(
    Guid Id,
    string? Name,
    bool IsGroup,
    MessageDto? LastMessage,
    int UnreadCount,
    List<ChatMemberDto> Members
);

public record ChatDetailsDto(
    Guid Id,
    string? Name,
    bool IsGroup,
    DateTime CreatedAt,
    List<ChatMemberDto> Members,
    int TotalMessages
);

public record CreateGroupChatDto(
    string Name,
    List<Guid> MemberIds
);
