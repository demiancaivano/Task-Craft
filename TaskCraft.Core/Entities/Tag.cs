namespace TaskCraft.Core.Entities;

public class Tag : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Color { get; set; }
    public Guid ProjectId { get; set; }

    public Project Project { get; set; } = null!;
    public ICollection<TaskTag> TaskTags { get; set; } = new List<TaskTag>();
}
