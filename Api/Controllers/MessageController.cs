using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Api.Filters;

namespace Api.Controllers;

[ApiController]
[Route("api/chats/{chatId:guid}/messages")]
[Authorize]
public class MessageController : ControllerBase
{
    private readonly IMessageService _messageService;

    public MessageController(IMessageService messageService)
    {
        _messageService = messageService;
    }

    // POST /api/chats/{chatId}/messages
    [HttpPost]
    [ExtractUserId]
    public async Task<ActionResult<MessageDto>> SendMessage(
        Guid chatId,
        [FromForm] SendMessageDto dto,
        [FromQuery] Guid userId)
    {
        try
        {
            var message = await _messageService.SendMessageAsync(chatId, userId, dto);
            return Ok(message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    // GET /api/chats/{chatId}/messages?page=1&pageSize=50
    [HttpGet]
    [ExtractUserId]
    public async Task<ActionResult<List<MessageDto>>> GetChatMessages(
        Guid chatId,
        [FromQuery] Guid userId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        try
        {
            var messages = await _messageService.GetChatMessagesAsync(chatId, userId, page, pageSize);
            return Ok(messages);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    // DELETE /api/messages/{messageId}
    [HttpDelete("/api/messages/{messageId:guid}")]
    [ExtractUserId]
    public async Task<IActionResult> DeleteMessage(
        Guid messageId,
        [FromQuery] Guid userId)
    {
        try
        {
            var result = await _messageService.DeleteMessageAsync(messageId, userId);
            if (!result)
            {
                return NotFound("Message not found");
            }
            return NoContent();
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    // GET /api/chats/{chatId}/unread-count
    [HttpGet("/api/chats/{chatId:guid}/unread-count")]
    [ExtractUserId]
    public async Task<ActionResult<int>> GetUnreadCount(
        Guid chatId,
        [FromQuery] Guid userId)
    {
        try
        {
            var count = await _messageService.GetUnreadCountAsync(userId, chatId);
            return Ok(count);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }
}
