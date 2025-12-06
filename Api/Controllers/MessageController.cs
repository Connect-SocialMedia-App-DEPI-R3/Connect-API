using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Api.Filters;
using Api.Hubs;

namespace Api.Controllers;

[ApiController]
[Route("api/chats/{chatId:guid}/messages")]
[Authorize]
public class MessageController : ControllerBase
{
    private readonly IMessageService _messageService;
    private readonly IHubContext<ChatHub> _hubContext;

    public MessageController(IMessageService messageService, IHubContext<ChatHub> hubContext)
    {
        _messageService = messageService;
        _hubContext = hubContext;
    }

    // POST /api/chats/{chatId}/messages
    [HttpPost]
    [ExtractUserId]
    public async Task<ActionResult<MessageDto>> SendMessage(
        Guid chatId,
        [FromForm] SendMessageDto dto)
    {
        var userId = (Guid)HttpContext.Items["UserId"]!;
        var message = await _messageService.SendMessageAsync(chatId, userId, dto);
                    // Broadcast message via SignalR
        await _hubContext.Clients.Group(chatId.ToString())
            .SendAsync("ReceiveMessage", message);
            
        return Ok(message);
    }

    // GET /api/chats/{chatId}/messages?page=1&pageSize=50
    [HttpGet]
    [ExtractUserId]
    public async Task<ActionResult<List<MessageDto>>> GetChatMessages(
        Guid chatId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        var userId = (Guid)HttpContext.Items["UserId"]!;
        var messages = await _messageService.GetChatMessagesAsync(chatId, userId, page, pageSize);
        return Ok(messages);
    }

    // DELETE /api/messages/{messageId}
    [HttpDelete("/api/messages/{messageId:guid}")]
    [ExtractUserId]
    public async Task<IActionResult> DeleteMessage(
        Guid messageId)
    {
        var userId = (Guid)HttpContext.Items["UserId"]!;
        var (success, chatId) = await _messageService.DeleteMessageAsync(messageId, userId);
        if (!success)
        {
            return NotFound("Message not found");
        }

        // Broadcast deletion via SignalR
        if (chatId.HasValue)
        {
            await _hubContext.Clients.Group(chatId.Value.ToString())
                .SendAsync("MessageDeleted", messageId);
        }
        
        return NoContent();
    }

    // GET /api/chats/{chatId}/unread-count
    [HttpGet("/api/chats/{chatId:guid}/unread-count")]
    [ExtractUserId]
    public async Task<ActionResult<int>> GetUnreadCount(
        Guid chatId)
    {
        var userId = (Guid)HttpContext.Items["UserId"]!;
        var count = await _messageService.GetUnreadCountAsync(userId, chatId);
        return Ok(count);
    }
}
