using Application.DTOs;
using Domain.Entities;

namespace Application.DTOs.Mappers;

public static class ChatDtoMapper
{
    public static ChatDto ToChatDto(this Chat chat, int unreadCount = 0)
    {
        var lastMessage = chat.Messages
            ?.OrderByDescending(m => m.CreatedAt)
            .FirstOrDefault();

        return new ChatDto(
            Id: chat.Id,
            Name: chat.Name,
            IsGroup: chat.IsGroup,
            LastMessage: lastMessage?.ToMessageDto(),
            UnreadCount: unreadCount,
            Members: chat.Members?.Select(m => m.ToChatMemberDto()).ToList() ?? new List<ChatMemberDto>()
        );
    }

    public static ChatDetailsDto ToChatDetailsDto(this Chat chat)
    {
        return new ChatDetailsDto(
            Id: chat.Id,
            Name: chat.Name,
            IsGroup: chat.IsGroup,
            CreatedAt: chat.CreatedAt,
            Members: chat.Members?.Select(m => m.ToChatMemberDto()).ToList() ?? new List<ChatMemberDto>(),
            TotalMessages: chat.Messages?.Count ?? 0
        );
    }
}
