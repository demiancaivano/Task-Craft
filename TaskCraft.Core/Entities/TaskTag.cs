namespace TaskCraft.Core.Entities;

public class TaskTag : BaseEntity
{
    public Guid TaskId { get; set; }
    public Guid TagId { get; set; }

    public Task Task { get; set; } = null!;
    public Tag Tag { get; set; } = null!;
}
