using Application.DTOs;

namespace Application.Interfaces;

public interface IMessageService
{
    Task<MessageDto> SendMessageAsync(Guid chatId, Guid userId, SendMessageDto dto);
    Task<List<MessageDto>> GetChatMessagesAsync(Guid chatId, Guid userId, int page, int pageSize);
    Task<(bool Success, Guid? ChatId)> DeleteMessageAsync(Guid messageId, Guid userId);
    Task<int> GetUnreadCountAsync(Guid userId, Guid chatId);
}
