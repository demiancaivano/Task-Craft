using TaskCraft.Core.Entities;

namespace TaskCraft.Core.Interfaces;

public interface IProjectRepository : IRepository<Project>
{
    Task<IEnumerable<Project>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Project>> GetProjectsByOwnerIdAsync(Guid ownerId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Project>> GetProjectsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Project?> GetProjectWithMembersAsync(Guid projectId, CancellationToken cancellationToken = default);
    Task<Project?> GetProjectWithTasksAsync(Guid projectId, CancellationToken cancellationToken = default);
    Task<Project?> GetProjectWithTagsAsync(Guid projectId, CancellationToken cancellationToken = default);
}
