namespace TaskCraft.Core.Entities;

public class Comment : BaseEntity
{
    public string Content { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public Guid? TaskId { get; set; }
    public Guid? ProjectId { get; set; }

    public User User { get; set; } = null!;
    public Task? Task { get; set; }
    public Project? Project { get; set; }
}
