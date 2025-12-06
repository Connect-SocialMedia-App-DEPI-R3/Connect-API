using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ChatRepository : IChatRepository
{
    private readonly AppDbContext _context;

    public ChatRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Chat?> GetByIdAsync(Guid chatId)
    {
        return await _context.Chats.FindAsync(chatId);
    }

    public async Task<Chat?> GetByIdWithMembersAsync(Guid chatId)
    {
        return await _context.Chats
            .Include(c => c.Members)
                .ThenInclude(m => m.User)
            .FirstOrDefaultAsync(c => c.Id == chatId);
    }

    public async Task<List<Chat>> GetUserChatsAsync(Guid userId)
    {
        return await _context.Chats
            .Where(c => c.Members.Any(m => m.UserId == userId))
            .Include(c => c.Members)
                .ThenInclude(m => m.User)
            .Include(c => c.Messages.OrderByDescending(m => m.CreatedAt).Take(1))
                .ThenInclude(m => m.Sender)
            .OrderByDescending(c => c.UpdatedAt)
            .ToListAsync();
    }

    public async Task<Chat?> GetPrivateChatAsync(Guid userId1, Guid userId2)
    {
        return await _context.Chats
            .Where(c => !c.IsGroup)
            .Where(c => c.Members.Count == 2 &&
                        c.Members.Any(m => m.UserId == userId1) &&
                        c.Members.Any(m => m.UserId == userId2))
            .Include(c => c.Members)
                .ThenInclude(m => m.User)
            .FirstOrDefaultAsync();
    }

    public async Task AddAsync(Chat chat)
    {
        await _context.Chats.AddAsync(chat);
    }

    public async Task UpdateAsync(Chat chat)
    {
        chat.UpdatedAt = DateTime.UtcNow;
        _context.Chats.Update(chat);
    }

    public async Task DeleteAsync(Chat chat)
    {
        _context.Chats.Remove(chat);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
