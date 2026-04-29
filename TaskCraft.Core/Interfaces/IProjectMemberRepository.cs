using TaskCraft.Core.Entities;

namespace TaskCraft.Core.Interfaces;

public interface IProjectMemberRepository : IRepository<ProjectMember>
{
    Task<ProjectMember?> GetByUserAndProjectAsync(Guid userId, Guid projectId, CancellationToken cancellationToken = default);
    Task<IEnumerable<ProjectMember>> GetByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default);
    Task<IEnumerable<ProjectMember>> GetMembersByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default);
    Task<IEnumerable<ProjectMember>> GetProjectsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<bool> IsUserMemberOfProjectAsync(Guid userId, Guid projectId, CancellationToken cancellationToken = default);
    Task<Enums.ProjectRole?> GetUserRoleInProjectAsync(Guid userId, Guid projectId, CancellationToken cancellationToken = default);
}
