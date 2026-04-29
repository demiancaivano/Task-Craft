using TaskCraft.Application.DTOs.Task;
using TaskCraft.Core.Entities;
using TaskCraft.Core.Enums;
using TaskEntity = TaskCraft.Core.Entities.Task;
using TaskStatus = TaskCraft.Core.Enums.TaskStatus;

namespace TaskCraft.Application.Interfaces;

/// <summary>
/// Service interface for task-related business operations
/// </summary>
public interface ITaskService
{
    Task<IEnumerable<TaskEntity>> GetAllTasksAsync(bool includeDeleted = false, CancellationToken cancellationToken = default);
    Task<TaskEntity?> GetTaskByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<TaskEntity?> GetTaskWithSubTasksAsync(Guid id, CancellationToken cancellationToken = default);
    Task<TaskEntity?> GetTaskWithAssignmentsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<TaskEntity>> GetTasksByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default);
    Task<IEnumerable<TaskEntity>> GetTasksByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<TaskEntity>> GetTasksByStatusAsync(Guid projectId, TaskStatus status, CancellationToken cancellationToken = default);
    Task<IEnumerable<TaskEntity>> GetOverdueTasksAsync(Guid projectId, CancellationToken cancellationToken = default);
    Task<TaskEntity> CreateTaskAsync(TaskEntity task, CancellationToken cancellationToken = default);
    Task<TaskEntity> CreateTaskAsync(CreateTaskDto createTaskDto, CancellationToken cancellationToken = default);
    Task<TaskEntity> UpdateTaskAsync(TaskEntity task, CancellationToken cancellationToken = default);
    Task<TaskEntity> UpdateTaskAsync(UpdateTaskDto updateTaskDto, CancellationToken cancellationToken = default);
    Task<bool> DeleteTaskAsync(Guid id, CancellationToken cancellationToken = default);
    Task<TaskAssignment> AssignUserToTaskAsync(Guid taskId, Guid userId, Guid? assignedBy = null, CancellationToken cancellationToken = default);
    Task<TaskAssignment> AssignUserToTaskAsync(AssignTaskDto assignTaskDto, CancellationToken cancellationToken = default);
    Task<bool> UnassignUserFromTaskAsync(Guid taskId, Guid userId, CancellationToken cancellationToken = default);
    Task<bool> UpdateTaskStatusAsync(Guid taskId, TaskStatus newStatus, CancellationToken cancellationToken = default);
    Task<bool> UpdateTaskPriorityAsync(Guid taskId, TaskPriority newPriority, CancellationToken cancellationToken = default);
}
