using Microsoft.AspNetCore.Identity;

namespace Domain.Entities;

public class User : IdentityUser<Guid>
{
    public string? AvatarUrl { get; set; } = null!;
    public string? Bio { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Posts
    public List<Post>? Posts { get; set; } = new List<Post>();
    public List<Comment>? Comments { get; set; } = new List<Comment>();
    public List<Reaction>? Reactions { get; set; } = new List<Reaction>();
    public List<Follow>? Followers { get; set; } = new List<Follow>();
    public List<Follow>? Following { get; set; } = new List<Follow>();
}
