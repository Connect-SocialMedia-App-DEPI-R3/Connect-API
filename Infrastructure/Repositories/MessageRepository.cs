using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class MessageRepository : IMessageRepository
{
    private readonly AppDbContext _context;

    public MessageRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Message?> GetByIdAsync(Guid messageId)
    {
        return await _context.Messages
            .Include(m => m.Sender)
            .FirstOrDefaultAsync(m => m.Id == messageId);
    }

    public async Task<List<Message>> GetChatMessagesAsync(Guid chatId, int page, int pageSize)
    {
        return await _context.Messages
            .Where(m => m.ChatId == chatId)
            .Include(m => m.Sender)
            .OrderByDescending(m => m.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> GetUnreadCountAsync(Guid userId, Guid chatId)
    {
        // This will be implemented later with read receipts
        // For now, return 0
        return await Task.FromResult(0);
    }

    public async Task AddAsync(Message message)
    {
        await _context.Messages.AddAsync(message);
    }

    public async Task DeleteAsync(Message message)
    {
        message.IsDeleted = true;
        message.UpdatedAt = DateTime.UtcNow;
        _context.Messages.Update(message);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
