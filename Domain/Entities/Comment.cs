namespace Domain.Entities;

public class Comment
{
    public Guid Id { get; set; }
    public string Content { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // User
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    // Post
    public Guid PostId { get; set; }
    public Post Post { get; set; } = null!;
}