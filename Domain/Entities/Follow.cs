namespace Domain.Entities;

public class Follow
{
    public int Id { get; set; }

    // Follower
    public string FollowerId { get; set; } = null!;
    public User Follower { get; set; } = null!;

    // Following
    public string FollowingId { get; set; } = null!;
    public User Following { get; set; } = null!;
}