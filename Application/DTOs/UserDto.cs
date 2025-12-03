using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

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
    [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Full name must contain only alphabetic characters and spaces.")]
    [MinLength(1, ErrorMessage = "Full name must have at least one character.")]
    [DefaultValue("Maryam Tarek")]
    public string? FullName { get; set; } = null!;
    [RegularExpression(@"^[a-zA-Z0-9_-]{3,20}$", ErrorMessage = "Username must be 3-20 characters long and can only contain letters, numbers, underscores, and hyphens.")]
    [DefaultValue("username123")]
    public string Username { get; set; } = null!;
    [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Invalid email format.")]
    [DefaultValue("person@example.com")]
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
    public int PostCount { get; set; }
    public string? AvatarUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
