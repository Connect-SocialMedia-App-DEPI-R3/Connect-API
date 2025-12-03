using System.ComponentModel.DataAnnotations;

namespace Application.DTOs;

public class CommentDto
{
    public Guid Id { get; set; }
    public Guid PostId { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public UserDto Author { get; set; } = null!;
}

public class CommentCreateDto
{
    [Required]
    [MinLength(1, ErrorMessage = "Comment cannot be empty.")]
    [MaxLength(500, ErrorMessage = "Comment cannot exceed 500 characters.")]
    public string Content { get; set; } = string.Empty;
}

public class CommentUpdateDto
{
    [Required]
    [MinLength(1, ErrorMessage = "Comment cannot be empty.")]
    [MaxLength(500, ErrorMessage = "Comment cannot exceed 500 characters.")]
    public string Content { get; set; } = string.Empty;
}