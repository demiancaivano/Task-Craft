using Microsoft.EntityFrameworkCore;
using TaskCraft.Core.Enums;
using TaskCraft.Core.Interfaces;
using TaskCraft.Infrastructure.Data;
using TaskEntity = TaskCraft.Core.Entities.Task;
using TaskStatus = TaskCraft.Core.Enums.TaskStatus;

namespace TaskCraft.Infrastructure.Repositories;

public class TaskRepository : Repository<TaskEntity>, ITaskRepository
{
    public TaskRepository(TaskCraftDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<TaskEntity>> GetByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        return await GetTasksByProjectIdAsync(projectId, cancellationToken);
    }

    public async Task<IEnumerable<TaskEntity>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await GetTasksByUserIdAsync(userId, cancellationToken);
    }

    public async Task<IEnumerable<TaskEntity>> GetByStatusAsync(TaskStatus status, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(t => t.Status == status)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TaskEntity>> GetOverdueTasksAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        return await _dbSet
            .Where(t => t.DueDate.HasValue 
                && t.DueDate < now 
                && t.Status != TaskStatus.Done)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TaskEntity>> GetTasksByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(t => t.ProjectId == projectId)
            .Include(t => t.Project)
            .Include(t => t.ParentTask)
            .Include(t => t.SubTasks)
            .Include(t => t.Comments)
            .Include(t => t.Assignments)
                .ThenInclude(a => a.User)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TaskEntity>> GetTasksByStatusAsync(Guid projectId, TaskStatus status, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(t => t.ProjectId == projectId && t.Status == status)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TaskEntity>> GetTasksByPriorityAsync(Guid projectId, TaskPriority priority, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(t => t.ProjectId == projectId && t.Priority == priority)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TaskEntity>> GetTasksByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(t => t.Assignments.Any(a => a.UserId == userId))
            .ToListAsync(cancellationToken);
    }

    public async Task<TaskEntity?> GetTaskWithSubTasksAsync(Guid taskId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(t => t.SubTasks)
            .FirstOrDefaultAsync(t => t.Id == taskId, cancellationToken);
    }

    public async Task<TaskEntity?> GetTaskWithAssignmentsAsync(Guid taskId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(t => t.Assignments)
                .ThenInclude(a => a.User)
            .FirstOrDefaultAsync(t => t.Id == taskId, cancellationToken);
    }

    public async Task<IEnumerable<TaskEntity>> GetSubTasksAsync(Guid parentTaskId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(t => t.ParentTaskId == parentTaskId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TaskEntity>> GetOverdueTasksAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        return await _dbSet
            .Where(t => t.ProjectId == projectId 
                && t.DueDate.HasValue 
                && t.DueDate < now 
                && t.Status != TaskStatus.Done)
            .ToListAsync(cancellationToken);
    }
}
