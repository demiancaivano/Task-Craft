namespace TaskCraft.Application.DTOs.Tag;

/// <summary>
/// DTO for returning tag information to the client
/// Used in GET requests
/// </summary>
public class TagDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Color { get; set; }
    public Guid ProjectId { get; set; }
    public int UsageCount { get; set; }
}
