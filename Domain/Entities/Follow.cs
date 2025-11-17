namespace Domain.Entities;

public class Follow
{
    public Guid Id { get; set; }

    // Follower
    public Guid FollowerId { get; set; }
    public User Follower { get; set; } = null!;

    // Following
    public Guid FollowingId { get; set; }
    public User Following { get; set; } = null!;
}