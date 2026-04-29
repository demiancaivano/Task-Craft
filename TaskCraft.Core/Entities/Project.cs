namespace TaskCraft.Core.Entities;

public class Project : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid OwnerId { get; set; }

    public User Owner { get; set; } = null!;
    public ICollection<ProjectMember> Members { get; set; } = new List<ProjectMember>();
    public ICollection<Task> Tasks { get; set; } = new List<Task>();
    public ICollection<Tag> Tags { get; set; } = new List<Tag>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
}
