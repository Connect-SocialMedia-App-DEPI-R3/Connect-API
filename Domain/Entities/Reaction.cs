namespace Domain.Entities;

public class Reaction
{
    public int Id { get; set; }
    public string? Type { get; set; } = "love"; // e.g., "Like", "Love", "Haha", etc.
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // User
    public string UserId { get; set; } = null!;
    public User User { get; set; } = null!;

    // Post
    public int PostId { get; set; }
    public Post Post { get; set; } = null!;
}