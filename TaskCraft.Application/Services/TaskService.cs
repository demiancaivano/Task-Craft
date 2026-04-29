using TaskCraft.Application.DTOs.Task;
using TaskCraft.Application.Interfaces;
using TaskCraft.Application.Mappings;
using TaskCraft.Core.Entities;
using TaskCraft.Core.Enums;
using TaskCraft.Core.Exceptions;
using TaskCraft.Core.Interfaces;
using TaskEntity = TaskCraft.Core.Entities.Task;
using TaskStatus = TaskCraft.Core.Enums.TaskStatus;

namespace TaskCraft.Application.Services;

/// <summary>
/// Service implementation for task-related business operations
/// </summary>
public class TaskService : ITaskService
{
    private readonly IUnitOfWork _unitOfWork;

    public TaskService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<TaskEntity>> GetAllTasksAsync(bool includeDeleted = false, CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.Tasks.GetAllAsync(includeDeleted, cancellationToken);
    }

    public async Task<TaskEntity?> GetTaskByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.Tasks.GetByIdAsync(id, cancellationToken);
    }

    public async Task<TaskEntity?> GetTaskWithSubTasksAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.Tasks.GetTaskWithSubTasksAsync(id, cancellationToken);
    }

    public async Task<TaskEntity?> GetTaskWithAssignmentsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.Tasks.GetTaskWithAssignmentsAsync(id, cancellationToken);
    }

    public async Task<IEnumerable<TaskEntity>> GetTasksByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.Tasks.GetTasksByProjectIdAsync(projectId, cancellationToken);
    }

    public async Task<IEnumerable<TaskEntity>> GetTasksByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.Tasks.GetTasksByUserIdAsync(userId, cancellationToken);
    }

    public async Task<IEnumerable<TaskEntity>> GetTasksByStatusAsync(Guid projectId, TaskStatus status, CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.Tasks.GetTasksByStatusAsync(projectId, status, cancellationToken);
    }

    public async Task<IEnumerable<TaskEntity>> GetOverdueTasksAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.Tasks.GetOverdueTasksAsync(projectId, cancellationToken);
    }

    public async Task<TaskEntity> CreateTaskAsync(TaskEntity task, CancellationToken cancellationToken = default)
    {
        // Business validation
        var validationErrors = new Dictionary<string, string[]>();

        if (string.IsNullOrWhiteSpace(task.Title))
            validationErrors.Add("Title", new[] { "Task title is required" });

        if (task.ProjectId == Guid.Empty)
            validationErrors.Add("ProjectId", new[] { "Project ID is required" });

        if (validationErrors.Any())
            throw new ValidationException(validationErrors);

        // Verify project exists
        var project = await _unitOfWork.Projects.GetByIdAsync(task.ProjectId, cancellationToken);
        if (project == null)
            throw new NotFoundException("Project", task.ProjectId);

        // Validate parent task if specified (for subtasks)
        if (task.ParentTaskId.HasValue)
        {
            var parentTask = await _unitOfWork.Tasks.GetByIdAsync(task.ParentTaskId.Value, cancellationToken);
            if (parentTask == null)
                throw new NotFoundException("Parent Task", task.ParentTaskId.Value);

            // Ensure parent task belongs to the same project
            if (parentTask.ProjectId != task.ProjectId)
                throw new BadRequestException("Subtask must belong to the same project as parent task");

            // Prevent nested subtasks (only 1 level)
            if (parentTask.ParentTaskId.HasValue)
                throw new BadRequestException("Cannot create subtask of a subtask. Only one level of subtasks is allowed");
        }

        // Validate dates
        if (task.StartDate.HasValue && task.DueDate.HasValue && task.StartDate > task.DueDate)
            throw new BadRequestException("Start date cannot be after due date");

        // Set default status if not specified
        if (task.Status == default)
            task.Status = TaskStatus.ToDo;

        // Set default priority if not specified
        if (task.Priority == default)
            task.Priority = TaskPriority.Medium;

        // Create task
        await _unitOfWork.Tasks.AddAsync(task, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return task;
    }

    public async System.Threading.Tasks.Task<TaskEntity> CreateTaskAsync(CreateTaskDto createTaskDto, CancellationToken cancellationToken = default)
    {
        var task = createTaskDto.ToEntity();
        return await CreateTaskAsync(task, cancellationToken);
    }

    public async Task<TaskEntity> UpdateTaskAsync(TaskEntity task, CancellationToken cancellationToken = default)
    {
        // Check if task exists
        var existingTask = await _unitOfWork.Tasks.GetByIdAsync(task.Id, cancellationToken);
        if (existingTask == null)
            throw new NotFoundException("Task", task.Id);

        // Business validation
        if (string.IsNullOrWhiteSpace(task.Title))
        {
            var errors = new Dictionary<string, string[]>
            {
                { "Title", new[] { "Task title is required" } }
            };
            throw new ValidationException(errors);
        }

        // Validate dates
        if (task.StartDate.HasValue && task.DueDate.HasValue && task.StartDate > task.DueDate)
            throw new BadRequestException("Start date cannot be after due date");

        // Update task
        await _unitOfWork.Tasks.UpdateAsync(task, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return task;
    }

    public async Task<TaskEntity> UpdateTaskAsync(UpdateTaskDto updateTaskDto, CancellationToken cancellationToken = default)
    {
        var existingTask = await _unitOfWork.Tasks.GetByIdAsync(updateTaskDto.Id, cancellationToken);
        if (existingTask == null)
            throw new NotFoundException("Task", updateTaskDto.Id);

        existingTask.Title = updateTaskDto.Title;
        existingTask.Description = updateTaskDto.Description;
        existingTask.Status = updateTaskDto.Status;
        existingTask.Priority = updateTaskDto.Priority;
        existingTask.StartDate = updateTaskDto.StartDate;
        existingTask.DueDate = updateTaskDto.DueDate;

        return await UpdateTaskAsync(existingTask, cancellationToken);
    }

    public async Task<bool> DeleteTaskAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var task = await _unitOfWork.Tasks.GetByIdAsync(id, cancellationToken);
        if (task == null)
            throw new NotFoundException("Task", id);

        // Check if task has subtasks
        var subTasks = await _unitOfWork.Tasks.GetSubTasksAsync(id, cancellationToken);
        if (subTasks.Any())
            throw new BadRequestException("Cannot delete task with subtasks. Delete subtasks first");

        // Soft delete
        await _unitOfWork.Tasks.SoftDeleteAsync(id, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<TaskAssignment> AssignUserToTaskAsync(Guid taskId, Guid userId, Guid? assignedBy = null, CancellationToken cancellationToken = default)
    {
        // Verify task exists
        var task = await _unitOfWork.Tasks.GetByIdAsync(taskId, cancellationToken);
        if (task == null)
            throw new NotFoundException("Task", taskId);

        // Verify user exists
        var user = await _unitOfWork.Users.GetByIdAsync(userId, cancellationToken);
        if (user == null)
            throw new NotFoundException("User", userId);

        // Check if user is already assigned
        var existingAssignment = await _unitOfWork.TaskAssignments.GetAssignmentAsync(userId, taskId, cancellationToken);
        if (existingAssignment != null)
            throw new ConflictException("User is already assigned to this task");

        // Verify user is a member of the project
        var isMember = await _unitOfWork.ProjectMembers.IsUserMemberOfProjectAsync(userId, task.ProjectId, cancellationToken);
        if (!isMember)
            throw new BadRequestException("User must be a member of the project to be assigned to tasks");

        // Create assignment
        var assignment = new TaskAssignment
        {
            TaskId = taskId,
            UserId = userId,
            AssignedBy = assignedBy,
            AssignedAt = DateTime.UtcNow
        };

        await _unitOfWork.TaskAssignments.AddAsync(assignment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return assignment;
    }

    public async System.Threading.Tasks.Task<TaskAssignment> AssignUserToTaskAsync(AssignTaskDto assignTaskDto, CancellationToken cancellationToken = default)
    {
        return await AssignUserToTaskAsync(assignTaskDto.TaskId, assignTaskDto.UserId, assignTaskDto.AssignedBy, cancellationToken);
    }

    public async Task<bool> UnassignUserFromTaskAsync(Guid taskId, Guid userId, CancellationToken cancellationToken = default)
    {
        // Get assignment
        var assignment = await _unitOfWork.TaskAssignments.GetAssignmentAsync(userId, taskId, cancellationToken);
        if (assignment == null)
            return false;

        // Remove assignment
        await _unitOfWork.TaskAssignments.DeleteAsync(assignment.Id, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> UpdateTaskStatusAsync(Guid taskId, TaskStatus newStatus, CancellationToken cancellationToken = default)
    {
        var task = await _unitOfWork.Tasks.GetByIdAsync(taskId, cancellationToken);
        if (task == null)
            throw new NotFoundException("Task", taskId);

        task.Status = newStatus;
        await _unitOfWork.Tasks.UpdateAsync(task, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> UpdateTaskPriorityAsync(Guid taskId, TaskPriority newPriority, CancellationToken cancellationToken = default)
    {
        var task = await _unitOfWork.Tasks.GetByIdAsync(taskId, cancellationToken);
        if (task == null)
            throw new NotFoundException("Task", taskId);

        task.Priority = newPriority;
        await _unitOfWork.Tasks.UpdateAsync(task, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
