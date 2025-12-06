using Domain.Entities;

namespace Domain.Interfaces;

public interface IMessageRepository
{
    Task<Message?> GetByIdAsync(Guid messageId);
    Task<List<Message>> GetChatMessagesAsync(Guid chatId, int page, int pageSize);
    Task<int> GetUnreadCountAsync(Guid userId, Guid chatId);
    Task AddAsync(Message message);
    Task DeleteAsync(Message message);
    Task SaveChangesAsync();
}
