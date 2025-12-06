using Application.DTOs;
using Application.DTOs.Mappers;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services;

public class ChatService : IChatService
{
    private readonly IChatRepository _chatRepository;
    private readonly IChatMemberRepository _chatMemberRepository;
    private readonly IUserRepository _userRepository;

    public ChatService(
        IChatRepository chatRepository,
        IChatMemberRepository chatMemberRepository,
        IUserRepository userRepository)
    {
        _chatRepository = chatRepository;
        _chatMemberRepository = chatMemberRepository;
        _userRepository = userRepository;
    }

    public async Task<ChatDto> CreatePrivateChatAsync(Guid userId1, Guid userId2)
    {
        // Check if private chat already exists
        var existingChat = await _chatRepository.GetPrivateChatAsync(userId1, userId2);
        if (existingChat != null)
        {
            return existingChat.ToChatDto();
        }

        // Verify both users exist
        var user1 = await _userRepository.GetByIdAsync(userId1);
        var user2 = await _userRepository.GetByIdAsync(userId2);
        
        if (user1 == null || user2 == null)
        {
            throw new ArgumentException("One or both users not found");
        }

        // Create new private chat
        var chat = new Chat
        {
            Id = Guid.NewGuid(),
            Name = null,
            IsGroup = false,
            Members = new List<ChatMember>
            {
                new ChatMember
                {
                    Id = Guid.NewGuid(),
                    UserId = userId1,
                    Role = ChatRole.Member
                },
                new ChatMember
                {
                    Id = Guid.NewGuid(),
                    UserId = userId2,
                    Role = ChatRole.Member
                }
            }
        };

        await _chatRepository.AddAsync(chat);
        await _chatRepository.SaveChangesAsync();

        var createdChat = await _chatRepository.GetByIdWithMembersAsync(chat.Id);
        return createdChat!.ToChatDto();
    }

    public async Task<ChatDto> CreateGroupChatAsync(Guid creatorId, CreateGroupChatDto dto)
    {
        if (dto.MemberIds.Count < 1)
        {
            throw new ArgumentException("Group chat requires at least 1 other member");
        }

        // Verify all users exist
        var allUserIds = dto.MemberIds.Append(creatorId).Distinct().ToList();
        foreach (var userId in allUserIds)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new ArgumentException($"User {userId} not found");
            }
        }

        // Create group chat
        var chat = new Chat
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            IsGroup = true,
            Members = new List<ChatMember>()
        };

        // Add creator as owner
        chat.Members.Add(new ChatMember
        {
            Id = Guid.NewGuid(),
            ChatId = chat.Id,
            UserId = creatorId,
            Role = ChatRole.Owner
        });

        // Add other members
        foreach (var memberId in dto.MemberIds.Where(id => id != creatorId))
        {
            chat.Members.Add(new ChatMember
            {
                Id = Guid.NewGuid(),
                ChatId = chat.Id,
                UserId = memberId,
                Role = ChatRole.Member
            });
        }

        await _chatRepository.AddAsync(chat);
        await _chatRepository.SaveChangesAsync();

        var createdChat = await _chatRepository.GetByIdWithMembersAsync(chat.Id);
        return createdChat!.ToChatDto();
    }

    public async Task<List<ChatDto>> GetUserChatsAsync(Guid userId)
    {
        var chats = await _chatRepository.GetUserChatsAsync(userId);
        return chats.Select(c => c.ToChatDto()).ToList();
    }

    public async Task<ChatDetailsDto> GetChatDetailsAsync(Guid chatId, Guid userId)
    {
        // Verify membership
        var isMember = await _chatMemberRepository.IsMemberAsync(chatId, userId);
        if (!isMember)
        {
            throw new UnauthorizedAccessException("You are not a member of this chat");
        }

        var chat = await _chatRepository.GetByIdWithMembersAsync(chatId);
        if (chat == null)
        {
            throw new ArgumentException("Chat not found");
        }

        return chat.ToChatDetailsDto();
    }

    public async Task<bool> DeleteChatAsync(Guid chatId, Guid userId)
    {
        var chat = await _chatRepository.GetByIdAsync(chatId);
        if (chat == null)
        {
            return false;
        }

        // Only owner can delete
        var role = await _chatMemberRepository.GetMemberRoleAsync(chatId, userId);
        if (role != ChatRole.Owner)
        {
            throw new UnauthorizedAccessException("Only the owner can delete the chat");
        }

        await _chatRepository.DeleteAsync(chat);
        await _chatRepository.SaveChangesAsync();
        return true;
    }

    public async Task<ChatDetailsDto> UpdateGroupNameAsync(Guid chatId, Guid userId, string newName)
    {
        var chat = await _chatRepository.GetByIdWithMembersAsync(chatId);
        if (chat == null)
        {
            throw new ArgumentException("Chat not found");
        }

        if (!chat.IsGroup)
        {
            throw new ArgumentException("Cannot update name of private chat");
        }

        // Only owner/admin can update name
        var role = await _chatMemberRepository.GetMemberRoleAsync(chatId, userId);
        if (role != ChatRole.Owner && role != ChatRole.Admin)
        {
            throw new UnauthorizedAccessException("Only owner or admin can update group name");
        }

        chat.Name = newName;
        await _chatRepository.UpdateAsync(chat);
        await _chatRepository.SaveChangesAsync();

        return chat.ToChatDetailsDto();
    }

    public async Task AddMembersAsync(Guid chatId, Guid userId, List<Guid> memberIds)
    {
        var chat = await _chatRepository.GetByIdAsync(chatId);
        if (chat == null)
        {
            throw new ArgumentException("Chat not found");
        }

        if (!chat.IsGroup)
        {
            throw new ArgumentException("Cannot add members to private chat");
        }

        // Only owner/admin can add members
        var role = await _chatMemberRepository.GetMemberRoleAsync(chatId, userId);
        if (role != ChatRole.Owner && role != ChatRole.Admin)
        {
            throw new UnauthorizedAccessException("Only owner or admin can add members");
        }

        // Add new members
        foreach (var memberId in memberIds)
        {
            // Check if already member
            var isMember = await _chatMemberRepository.IsMemberAsync(chatId, memberId);
            if (isMember) continue;

            // Verify user exists
            var user = await _userRepository.GetByIdAsync(memberId);
            if (user == null) continue;

            var newMember = new ChatMember
            {
                Id = Guid.NewGuid(),
                ChatId = chatId,
                UserId = memberId,
                Role = ChatRole.Member
            };

            await _chatMemberRepository.AddAsync(newMember);
        }

        await _chatMemberRepository.SaveChangesAsync();
    }

    public async Task RemoveMemberAsync(Guid chatId, Guid userId, Guid memberToRemoveId)
    {
        var chat = await _chatRepository.GetByIdAsync(chatId);
        if (chat == null)
        {
            throw new ArgumentException("Chat not found");
        }

        if (!chat.IsGroup)
        {
            throw new ArgumentException("Cannot remove members from private chat");
        }

        // Only owner/admin can remove members
        var role = await _chatMemberRepository.GetMemberRoleAsync(chatId, userId);
        if (role != ChatRole.Owner && role != ChatRole.Admin)
        {
            throw new UnauthorizedAccessException("Only owner or admin can remove members");
        }

        // Cannot remove owner
        var targetRole = await _chatMemberRepository.GetMemberRoleAsync(chatId, memberToRemoveId);
        if (targetRole == ChatRole.Owner)
        {
            throw new ArgumentException("Cannot remove the owner");
        }

        var member = await _chatMemberRepository.GetMemberAsync(chatId, memberToRemoveId);
        if (member == null)
        {
            throw new ArgumentException("Member not found");
        }

        await _chatMemberRepository.RemoveAsync(member);
        await _chatMemberRepository.SaveChangesAsync();
    }

    public async Task LeaveGroupAsync(Guid chatId, Guid userId)
    {
        var chat = await _chatRepository.GetByIdAsync(chatId);
        if (chat == null)
        {
            throw new ArgumentException("Chat not found");
        }

        if (!chat.IsGroup)
        {
            throw new ArgumentException("Cannot leave private chat");
        }

        // Owner cannot leave without transferring ownership
        var role = await _chatMemberRepository.GetMemberRoleAsync(chatId, userId);
        if (role == ChatRole.Owner)
        {
            throw new ArgumentException("Owner cannot leave. Transfer ownership or delete the group");
        }

        var member = await _chatMemberRepository.GetMemberAsync(chatId, userId);
        if (member == null)
        {
            throw new ArgumentException("You are not a member of this chat");
        }

        await _chatMemberRepository.RemoveAsync(member);
        await _chatMemberRepository.SaveChangesAsync();
    }

    public async Task UpdateMemberRoleAsync(Guid chatId, Guid userId, Guid targetUserId, ChatRole newRole)
    {
        // Only owner can promote to admin
        var role = await _chatMemberRepository.GetMemberRoleAsync(chatId, userId);
        if (role != ChatRole.Owner)
        {
            throw new UnauthorizedAccessException("Only owner can update member roles");
        }

        if (newRole == ChatRole.Owner)
        {
            throw new ArgumentException("Cannot promote to owner. Use transfer ownership instead");
        }

        var member = await _chatMemberRepository.GetMemberAsync(chatId, targetUserId);
        if (member == null)
        {
            throw new ArgumentException("Member not found");
        }

        member.Role = newRole;
        await _chatMemberRepository.UpdateAsync(member);
        await _chatMemberRepository.SaveChangesAsync();
    }
}
