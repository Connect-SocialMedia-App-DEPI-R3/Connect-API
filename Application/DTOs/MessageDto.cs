using Microsoft.AspNetCore.Http;

namespace Application.DTOs;

public record MessageDto(
    Guid Id,
    Guid ChatId,
    UserDto Sender,
    string Content,
    string? AttachmentUrl,
    DateTime CreatedAt,
    bool IsDeleted
);

public record SendMessageDto(
    string Content,
    IFormFile? Attachment
);
