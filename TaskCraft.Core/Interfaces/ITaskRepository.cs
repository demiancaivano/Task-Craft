using TaskCraft.Core.Entities;
using TaskEntity = TaskCraft.Core.Entities.Task;

namespace TaskCraft.Core.Interfaces;

public interface ITaskRepository : IRepository<TaskEntity>
{
    System.Threading.Tasks.Task<IEnumerable<TaskEntity>> GetByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default);
    System.Threading.Tasks.Task<IEnumerable<TaskEntity>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    System.Threading.Tasks.Task<IEnumerable<TaskEntity>> GetByStatusAsync(Enums.TaskStatus status, CancellationToken cancellationToken = default);
    System.Threading.Tasks.Task<IEnumerable<TaskEntity>> GetOverdueTasksAsync(CancellationToken cancellationToken = default);
    System.Threading.Tasks.Task<IEnumerable<TaskEntity>> GetTasksByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default);
    System.Threading.Tasks.Task<IEnumerable<TaskEntity>> GetTasksByStatusAsync(Guid projectId, Enums.TaskStatus status, CancellationToken cancellationToken = default);
    System.Threading.Tasks.Task<IEnumerable<TaskEntity>> GetTasksByPriorityAsync(Guid projectId, Enums.TaskPriority priority, CancellationToken cancellationToken = default);
    System.Threading.Tasks.Task<IEnumerable<TaskEntity>> GetTasksByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    System.Threading.Tasks.Task<TaskEntity?> GetTaskWithSubTasksAsync(Guid taskId, CancellationToken cancellationToken = default);
    System.Threading.Tasks.Task<TaskEntity?> GetTaskWithAssignmentsAsync(Guid taskId, CancellationToken cancellationToken = default);
    System.Threading.Tasks.Task<IEnumerable<TaskEntity>> GetSubTasksAsync(Guid parentTaskId, CancellationToken cancellationToken = default);
    System.Threading.Tasks.Task<IEnumerable<TaskEntity>> GetOverdueTasksAsync(Guid projectId, CancellationToken cancellationToken = default);
}
