using Microsoft.EntityFrameworkCore;
using TaskCraft.Core.Entities;
using TaskCraft.Core.Interfaces;
using TaskCraft.Infrastructure.Data;

namespace TaskCraft.Infrastructure.Repositories;

public class CommentRepository : Repository<Comment>, ICommentRepository
{
    public CommentRepository(TaskCraftDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Comment>> GetCommentsByTaskIdAsync(Guid taskId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(c => c.User)
            .Where(c => c.TaskId == taskId)
            .OrderBy(c => c.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Comment>> GetCommentsByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(c => c.User)
            .Where(c => c.ProjectId == projectId)
            .OrderBy(c => c.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Comment>> GetCommentsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(c => c.UserId == userId)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetCommentCountByTaskIdAsync(Guid taskId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .CountAsync(c => c.TaskId == taskId, cancellationToken);
    }

    public async Task<int> GetCommentCountByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .CountAsync(c => c.ProjectId == projectId, cancellationToken);
    }
}
