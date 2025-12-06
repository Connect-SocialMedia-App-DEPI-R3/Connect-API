using Domain.Entities;

namespace Domain.Interfaces;

public interface IChatRepository
{
    Task<Chat?> GetByIdAsync(Guid chatId);
    Task<Chat?> GetByIdWithMembersAsync(Guid chatId);
    Task<List<Chat>> GetUserChatsAsync(Guid userId);
    Task<Chat?> GetPrivateChatAsync(Guid userId1, Guid userId2);
    Task AddAsync(Chat chat);
    Task UpdateAsync(Chat chat);
    Task DeleteAsync(Chat chat);
    Task SaveChangesAsync();
}
