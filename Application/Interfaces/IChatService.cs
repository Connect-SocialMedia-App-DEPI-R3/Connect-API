using Application.DTOs;
using Domain.Entities;

namespace Application.Interfaces;

public interface IChatService
{
    // Chat Management
    Task<ChatDto> CreatePrivateChatAsync(Guid userId1, Guid userId2);
    Task<ChatDto> CreateGroupChatAsync(Guid creatorId, CreateGroupChatDto dto);
    Task<List<ChatDto>> GetUserChatsAsync(Guid userId);
    Task<ChatDetailsDto> GetChatDetailsAsync(Guid chatId, Guid userId);
    Task<bool> DeleteChatAsync(Guid chatId, Guid userId);

    // Group Management
    Task<ChatDetailsDto> UpdateGroupNameAsync(Guid chatId, Guid userId, string newName);
    Task AddMembersAsync(Guid chatId, Guid userId, List<Guid> memberIds);
    Task RemoveMemberAsync(Guid chatId, Guid userId, Guid memberToRemoveId);
    Task LeaveGroupAsync(Guid chatId, Guid userId);
    Task UpdateMemberRoleAsync(Guid chatId, Guid userId, Guid targetUserId, ChatRole newRole);
}
