using Application.DTOs;
using Domain.Entities;

namespace Application.DTOs.Mappers;

public static class ChatMemberDtoMapper
{
    public static ChatMemberDto ToChatMemberDto(this ChatMember member)
    {
        return new ChatMemberDto(
            User: new UserDto(
                Id: member.User.Id,
                FullName: member.User.FullName,
                Username: member.User.UserName ?? string.Empty,
                AvatarUrl: member.User.AvatarUrl
            ),
            Role: member.Role,
            JoinedAt: member.JoinedAt
        );
    }
}
