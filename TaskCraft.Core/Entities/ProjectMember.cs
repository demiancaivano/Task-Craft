using TaskCraft.Core.Enums;

namespace TaskCraft.Core.Entities;

public class ProjectMember : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid ProjectId { get; set; }
    public ProjectRole Role { get; set; }
    public DateTime JoinedAt { get; set; }

    public User User { get; set; } = null!;
    public Project Project { get; set; } = null!;

    public ProjectMember()
    {
        JoinedAt = DateTime.UtcNow;
    }
}
