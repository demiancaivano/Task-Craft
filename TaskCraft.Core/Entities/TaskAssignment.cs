namespace TaskCraft.Core.Entities;

public class TaskAssignment : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid TaskId { get; set; }
    public DateTime AssignedAt { get; set; }
    public Guid? AssignedBy { get; set; }

    public User User { get; set; } = null!;
    public Task Task { get; set; } = null!;

    public TaskAssignment()
    {
        AssignedAt = DateTime.UtcNow;
    }
}
