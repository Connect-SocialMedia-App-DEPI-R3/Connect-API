using Application.DTOs;
using Domain.Entities;

namespace Application.DTOs.Mappers;

public static class MessageDtoMapper
{
    public static MessageDto ToMessageDto(this Message message)
    {
        return new MessageDto(
            Id: message.Id,
            ChatId: message.ChatId,
            Sender: new UserDto(
                Id: message.Sender.Id,
                FullName: message.Sender.FullName,
                Username: message.Sender.UserName ?? string.Empty,
                AvatarUrl: message.Sender.AvatarUrl
            ),
            Content: message.IsDeleted ? "Message deleted" : message.Content,
            AttachmentUrl: message.IsDeleted ? null : message.AttachmentUrl,
            CreatedAt: message.CreatedAt,
            IsDeleted: message.IsDeleted
        );
    }
}
