namespace Application.DTOs;

public record UserDto(Guid Id, string? FullName, string Username, string? AvatarUrl);
public class UserDetailedDto
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Bio { get; set; }
    public string? AvatarUrl { get; set; }
    public int FollowerCount { get; set; }
    public int FollowingCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<PostSimpleViewDto> Posts { get; set; } = new();
}
public class UserRegisterDto
{
    public string? FullName { get; set; } = null!;
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
}

public class UserLoginDto
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
}

public class UserUpdateProfileDto
{
    public string? FullName { get; set; } = string.Empty;
    public string? Username { get; set; }
    public string? Bio { get; set; }
}

public class UserProfileDto
{
    public Guid Id { get; set; }
    public string? FullName { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Bio { get; set; }
    public int FollowerCount { get; set; }
    public int FollowingCount { get; set; }
    public string? AvatarUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
