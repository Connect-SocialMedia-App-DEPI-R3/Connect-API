using Domain.Entities;

namespace Domain.Interfaces;

public interface IChatMemberRepository
{
    Task<ChatMember?> GetMemberAsync(Guid chatId, Guid userId);
    Task<List<ChatMember>> GetChatMembersAsync(Guid chatId);
    Task<ChatRole?> GetMemberRoleAsync(Guid chatId, Guid userId);
    Task<bool> IsMemberAsync(Guid chatId, Guid userId);
    Task AddAsync(ChatMember member);
    Task RemoveAsync(ChatMember member);
    Task UpdateAsync(ChatMember member);
    Task SaveChangesAsync();
}
