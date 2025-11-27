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
    public string Content { get; set; } = string.Empty;
}

public class CommentUpdateDto
{
    public string Content { get; set; } = string.Empty;
}