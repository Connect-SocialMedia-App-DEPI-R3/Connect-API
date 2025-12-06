namespace Domain.Entities;

public class ChatMember
{
    public Guid Id { get; set; }
    public Guid ChatId { get; set; }
    public Chat Chat { get; set; } = null!;

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public ChatRole Role { get; set; }  // Owner, Admin, Member

    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
