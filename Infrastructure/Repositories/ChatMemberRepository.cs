using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ChatMemberRepository : IChatMemberRepository
{
    private readonly AppDbContext _context;

    public ChatMemberRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ChatMember?> GetMemberAsync(Guid chatId, Guid userId)
    {
        return await _context.ChatMembers
            .Include(cm => cm.User)
            .FirstOrDefaultAsync(cm => cm.ChatId == chatId && cm.UserId == userId);
    }

    public async Task<List<ChatMember>> GetChatMembersAsync(Guid chatId)
    {
        return await _context.ChatMembers
            .Where(cm => cm.ChatId == chatId)
            .Include(cm => cm.User)
            .ToListAsync();
    }

    public async Task<ChatRole?> GetMemberRoleAsync(Guid chatId, Guid userId)
    {
        var member = await _context.ChatMembers
            .FirstOrDefaultAsync(cm => cm.ChatId == chatId && cm.UserId == userId);
        
        return member?.Role;
    }

    public async Task<bool> IsMemberAsync(Guid chatId, Guid userId)
    {
        return await _context.ChatMembers
            .AnyAsync(cm => cm.ChatId == chatId && cm.UserId == userId);
    }

    public async Task AddAsync(ChatMember member)
    {
        await _context.ChatMembers.AddAsync(member);
    }

    public async Task RemoveAsync(ChatMember member)
    {
        _context.ChatMembers.Remove(member);
    }

    public async Task UpdateAsync(ChatMember member)
    {
        member.UpdatedAt = DateTime.UtcNow;
        _context.ChatMembers.Update(member);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
