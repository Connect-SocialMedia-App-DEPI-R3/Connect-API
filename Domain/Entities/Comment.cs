namespace Domain.Entities;

public class Comment
{
    public int Id { get; set; }
    public string Content { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // User
    public string UserId { get; set; } = null!;
    public User User { get; set; } = null!;

    // Post
    public int PostId { get; set; }
    public Post Post { get; set; } = null!;
}