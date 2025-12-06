namespace Domain.Entities;

public class Chat
{
    public Guid Id { get; set; }
    public string? Name { get; set; }  // null for private chats
    public bool IsGroup { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public List<Message> Messages { get; set; } = new List<Message>();
    public List<ChatMember> Members { get; set; } = new List<ChatMember>();
}
