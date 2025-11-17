namespace Domain.Entities;

public class Reaction
{
    public Guid Id { get; set; }
    public string? Type { get; set; } = "love"; // e.g., "Like", "Love", "Haha", etc.
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // User
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    // Post
    public Guid PostId { get; set; }
    public Post Post { get; set; } = null!;
}