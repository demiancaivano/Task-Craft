using Microsoft.EntityFrameworkCore;
using TaskCraft.Core.Entities;
using TaskCraft.Core.Interfaces;
using TaskCraft.Infrastructure.Data;

namespace TaskCraft.Infrastructure.Repositories;

public class ProjectRepository : Repository<Project>, IProjectRepository
{
    public ProjectRepository(TaskCraftDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Project>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await GetProjectsByUserIdAsync(userId, cancellationToken);
    }

    public async Task<IEnumerable<Project>> GetProjectsByOwnerIdAsync(Guid ownerId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(p => p.OwnerId == ownerId)
            .ToListAsync(cancellationToken);
    }

    public override async Task<Project?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.Owner)
            .Include(p => p.Members)
                .ThenInclude(m => m.User)
            .Include(p => p.Tasks)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Project>> GetProjectsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(p => p.OwnerId == userId || p.Members.Any(m => m.UserId == userId && !m.IsDeleted))
            .Include(p => p.Owner)
            .Include(p => p.Members)
            .Include(p => p.Tasks)
            .ToListAsync(cancellationToken);
    }

    public async Task<Project?> GetProjectWithMembersAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.Members)
                .ThenInclude(m => m.User)
            .FirstOrDefaultAsync(p => p.Id == projectId, cancellationToken);
    }

    public async Task<Project?> GetProjectWithTasksAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.Tasks)
            .FirstOrDefaultAsync(p => p.Id == projectId, cancellationToken);
    }

    public async Task<Project?> GetProjectWithTagsAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.Tags)
            .FirstOrDefaultAsync(p => p.Id == projectId, cancellationToken);
    }
}
