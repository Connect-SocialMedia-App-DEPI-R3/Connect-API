using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Api.Filters;

namespace Api.Controllers;

[ApiController]
[Route("api/chats")]
[Authorize]
public class ChatController : ControllerBase
{
    private readonly IChatService _chatService;

    public ChatController(IChatService chatService)
    {
        _chatService = chatService;
    }

    // POST /api/chats/private/{targetUserId}
    [HttpPost("private/{targetUserId:guid}")]
    [ExtractUserId]
    public async Task<ActionResult<ChatDto>> CreatePrivateChat(
        Guid targetUserId)
    {
        var userId = (Guid)HttpContext.Items["UserId"]!;
        var chat = await _chatService.CreatePrivateChatAsync(userId, targetUserId);
        return Ok(chat);
    }

    // POST /api/chats/group
    [HttpPost("group")]
    [ExtractUserId]
    public async Task<ActionResult<ChatDto>> CreateGroupChat(
        [FromBody] CreateGroupChatDto dto)
    {
        var userId = (Guid)HttpContext.Items["UserId"]!;
        var chat = await _chatService.CreateGroupChatAsync(userId, dto);
        return Ok(chat);
    }

    // GET /api/chats
    [HttpGet]
    [ExtractUserId]
    public async Task<ActionResult<List<ChatDto>>> GetUserChats()
    {
        var userId = (Guid)HttpContext.Items["UserId"]!;
        var chats = await _chatService.GetUserChatsAsync(userId);
        return Ok(chats);
    }

    // GET /api/chats/{chatId}
    [HttpGet("{chatId:guid}")]
    [ExtractUserId]
    public async Task<ActionResult<ChatDetailsDto>> GetChatDetails(
        Guid chatId)
    {
        var userId = (Guid)HttpContext.Items["UserId"]!;
        var chat = await _chatService.GetChatDetailsAsync(chatId, userId);
        return Ok(chat);
    }

    // PUT /api/chats/{chatId}/name
    [HttpPut("{chatId:guid}/name")]
    [ExtractUserId]
    public async Task<ActionResult<ChatDetailsDto>> UpdateGroupName(
        Guid chatId,
        [FromBody] string newName)
    {
        var userId = (Guid)HttpContext.Items["UserId"]!;
        var chat = await _chatService.UpdateGroupNameAsync(chatId, userId, newName);
        return Ok(chat);
    }

    // DELETE /api/chats/{chatId}
    [HttpDelete("{chatId:guid}")]
    [ExtractUserId]
    public async Task<IActionResult> DeleteChat(
        Guid chatId)
    {
        var userId = (Guid)HttpContext.Items["UserId"]!;
        var result = await _chatService.DeleteChatAsync(chatId, userId);
        if (!result)
        {
            return NotFound("Chat not found");
        }
        return NoContent();
    }

    // POST /api/chats/{chatId}/members
    [HttpPost("{chatId:guid}/members")]
    [ExtractUserId]
    public async Task<IActionResult> AddMembers(
        Guid chatId,
        [FromBody] List<Guid> memberIds)
    {
        var userId = (Guid)HttpContext.Items["UserId"]!;
        await _chatService.AddMembersAsync(chatId, userId, memberIds);
        return Ok();
    }

    // DELETE /api/chats/{chatId}/members/{memberUserId}
    [HttpDelete("{chatId:guid}/members/{memberUserId:guid}")]
    [ExtractUserId]
    public async Task<IActionResult> RemoveMember(
        Guid chatId,
        Guid memberUserId)
    {
        var userId = (Guid)HttpContext.Items["UserId"]!;
        await _chatService.RemoveMemberAsync(chatId, userId, memberUserId);
        return NoContent();
    }

    // POST /api/chats/{chatId}/leave
    [HttpPost("{chatId:guid}/leave")]
    [ExtractUserId]
    public async Task<IActionResult> LeaveGroup(
        Guid chatId)
    {
        var userId = (Guid)HttpContext.Items["UserId"]!;
        await _chatService.LeaveGroupAsync(chatId, userId);
        return NoContent();
    }

    // PUT /api/chats/{chatId}/members/{memberUserId}/role
    [HttpPut("{chatId:guid}/members/{memberUserId:guid}/role")]
    [ExtractUserId]
    public async Task<IActionResult> UpdateMemberRole(
        Guid chatId,
        Guid memberUserId,
        [FromBody] ChatRole newRole)
    {
        var userId = (Guid)HttpContext.Items["UserId"]!;
        await _chatService.UpdateMemberRoleAsync(chatId, userId, memberUserId, newRole);
        return Ok();
    }
}
