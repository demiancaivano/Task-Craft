using Microsoft.EntityFrameworkCore;
using TaskCraft.Core.Entities;
using TaskCraft.Core.Enums;
using TaskCraft.Core.Interfaces;
using TaskCraft.Infrastructure.Data;

namespace TaskCraft.Infrastructure.Repositories;

public class ProjectMemberRepository : Repository<ProjectMember>, IProjectMemberRepository
{
    public ProjectMemberRepository(TaskCraftDbContext context) : base(context)
    {
    }

    public async Task<ProjectMember?> GetByUserAndProjectAsync(Guid userId, Guid projectId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(pm => pm.UserId == userId && pm.ProjectId == projectId, cancellationToken);
    }

    public async Task<IEnumerable<ProjectMember>> GetByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        return await GetMembersByProjectIdAsync(projectId, cancellationToken);
    }

    public async Task<IEnumerable<ProjectMember>> GetMembersByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(pm => pm.User)
            .Where(pm => pm.ProjectId == projectId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ProjectMember>> GetProjectsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(pm => pm.Project)
            .Where(pm => pm.UserId == userId)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> IsUserMemberOfProjectAsync(Guid userId, Guid projectId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AnyAsync(pm => pm.UserId == userId && pm.ProjectId == projectId, cancellationToken);
    }

    public async Task<ProjectRole?> GetUserRoleInProjectAsync(Guid userId, Guid projectId, CancellationToken cancellationToken = default)
    {
        var member = await _dbSet
            .FirstOrDefaultAsync(pm => pm.UserId == userId && pm.ProjectId == projectId, cancellationToken);

        return member?.Role;
    }
}
