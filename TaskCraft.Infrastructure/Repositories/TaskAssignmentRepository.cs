using Microsoft.EntityFrameworkCore;
using TaskCraft.Core.Entities;
using TaskCraft.Core.Interfaces;
using TaskCraft.Infrastructure.Data;

namespace TaskCraft.Infrastructure.Repositories;

public class TaskAssignmentRepository : Repository<TaskAssignment>, ITaskAssignmentRepository
{
    public TaskAssignmentRepository(TaskCraftDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<TaskAssignment>> GetByTaskIdAsync(Guid taskId, CancellationToken cancellationToken = default)
    {
        return await GetAssignmentsByTaskIdAsync(taskId, cancellationToken);
    }

    public async Task<IEnumerable<TaskAssignment>> GetAssignmentsByTaskIdAsync(Guid taskId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(ta => ta.User)
            .Where(ta => ta.TaskId == taskId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TaskAssignment>> GetAssignmentsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(ta => ta.Task)
                .ThenInclude(t => t.Project)
            .Where(ta => ta.UserId == userId)
            .ToListAsync(cancellationToken);
    }

    public async Task<TaskAssignment?> GetAssignmentAsync(Guid userId, Guid taskId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(ta => ta.UserId == userId && ta.TaskId == taskId, cancellationToken);
    }

    public async Task<bool> IsUserAssignedToTaskAsync(Guid userId, Guid taskId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AnyAsync(ta => ta.UserId == userId && ta.TaskId == taskId, cancellationToken);
    }

    public async Task<int> GetAssignmentCountByTaskIdAsync(Guid taskId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .CountAsync(ta => ta.TaskId == taskId, cancellationToken);
    }
}
