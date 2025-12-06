using Application.DTOs;
using Application.DTOs.Mappers;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services;

public class MessageService : IMessageService
{
    private readonly IMessageRepository _messageRepository;
    private readonly IChatMemberRepository _chatMemberRepository;
    private readonly IChatRepository _chatRepository;
    private readonly IImageService _imageService;

    public MessageService(
        IMessageRepository messageRepository,
        IChatMemberRepository chatMemberRepository,
        IChatRepository chatRepository,
        IImageService imageService)
    {
        _messageRepository = messageRepository;
        _chatMemberRepository = chatMemberRepository;
        _chatRepository = chatRepository;
        _imageService = imageService;
    }

    public async Task<MessageDto> SendMessageAsync(Guid chatId, Guid senderId, SendMessageDto dto)
    {
        // Verify chat exists
        var chat = await _chatRepository.GetByIdAsync(chatId);
        if (chat == null)
        {
            throw new ArgumentException("Chat not found");
        }

        // Verify membership
        var isMember = await _chatMemberRepository.IsMemberAsync(chatId, senderId);
        if (!isMember)
        {
            throw new UnauthorizedAccessException("You are not a member of this chat");
        }

        // Upload attachment if provided
        string? attachmentUrl = null;
        if (dto.Attachment != null)
        {
            using var stream = dto.Attachment.OpenReadStream();
            attachmentUrl = await _imageService.UploadImageAsync(
                stream,
                dto.Attachment.FileName,
                dto.Attachment.ContentType
            );
        }

        // Create message
        var message = new Message
        {
            Id = Guid.NewGuid(),
            ChatId = chatId,
            SenderId = senderId,
            Content = dto.Content,
            AttachmentUrl = attachmentUrl
        };

        await _messageRepository.AddAsync(message);
        
        // Update chat's UpdatedAt
        chat.UpdatedAt = DateTime.UtcNow;
        await _chatRepository.UpdateAsync(chat);
        
        await _messageRepository.SaveChangesAsync();

        // Retrieve message with sender info
        var createdMessage = await _messageRepository.GetByIdAsync(message.Id);
        return createdMessage!.ToMessageDto();
    }

    public async Task<List<MessageDto>> GetChatMessagesAsync(Guid chatId, Guid userId, int page, int pageSize)
    {
        // Verify membership
        var isMember = await _chatMemberRepository.IsMemberAsync(chatId, userId);
        if (!isMember)
        {
            throw new UnauthorizedAccessException("You are not a member of this chat");
        }

        var messages = await _messageRepository.GetChatMessagesAsync(chatId, page, pageSize);
        return messages.Select(m => m.ToMessageDto()).ToList();
    }

    public async Task<bool> DeleteMessageAsync(Guid messageId, Guid userId)
    {
        var message = await _messageRepository.GetByIdAsync(messageId);
        if (message == null)
        {
            return false;
        }

        // Only sender can delete their message
        if (message.SenderId != userId)
        {
            throw new UnauthorizedAccessException("You can only delete your own messages");
        }

        await _messageRepository.DeleteAsync(message);
        await _messageRepository.SaveChangesAsync();
        return true;
    }

    public async Task<int> GetUnreadCountAsync(Guid userId, Guid chatId)
    {
        // Verify membership
        var isMember = await _chatMemberRepository.IsMemberAsync(chatId, userId);
        if (!isMember)
        {
            throw new UnauthorizedAccessException("You are not a member of this chat");
        }

        return await _messageRepository.GetUnreadCountAsync(userId, chatId);
    }
}
