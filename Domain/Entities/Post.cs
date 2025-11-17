namespace Domain.Entities;

public class Post
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string Content { get; set; } = null!;
    public string? ImageUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // User
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public List<Comment>? Comments { get; set; } = new List<Comment>();
    public List<Reaction>? Reactions { get; set; } = new List<Reaction>();
}