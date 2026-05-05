using TaskCraft.Application.DTOs.Task;
using TaskCraft.Core.Entities;
using TaskStatus = TaskCraft.Core.Enums.TaskStatus;

namespace TaskCraft.Application.Mappings;

/// <summary>
/// Extension methods for mapping between Task entity and Task DTOs
/// </summary>
public static class TaskMappingExtensions
{
    /// <summary>
    /// Converts Task entity to TaskDto
    /// Calculates IsOverdue, counts related entities, and denormalizes related names
    /// </summary>
    public static TaskDto ToDto(this Core.Entities.Task task)
    {
        return new TaskDto
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            Status = task.Status,
            Priority = task.Priority,
            StartDate = task.StartDate,
            DueDate = task.DueDate,
            ProjectId = task.ProjectId,
            ProjectName = task.Project?.Name ?? "Unknown",
            ParentTaskId = task.ParentTaskId,
            ParentTaskTitle = task.ParentTask?.Title,
            CreatedAt = task.CreatedAt,
            IsOverdue = task.DueDate.HasValue &&
                       task.DueDate.Value < DateTime.UtcNow && 
                       task.Status != TaskStatus.Done && 
                       task.Status != TaskStatus.Blocked,
            AssigneeCount = task.Assignments?.Count(a => !a.IsDeleted) ?? 0,
            SubTaskCount = task.SubTasks?.Count(st => !st.IsDeleted) ?? 0,
            CommentCount = task.Comments?.Count(c => !c.IsDeleted) ?? 0,
            HasSubTasks = task.SubTasks?.Any(st => !st.IsDeleted) ?? false,
            Assignees = task.Assignments?
                .Where(a => !a.IsDeleted && a.User != null)
                .Select(a => new TaskAssigneeDto { UserId = a.UserId, Username = a.User!.Username })
                .ToList() ?? []
        };
    }

    /// <summary>
    /// Converts CreateTaskDto to Task entity
    /// Validation of ParentTaskId, ProjectId, and dates should be done in service layer
    /// </summary>
    public static Core.Entities.Task ToEntity(this CreateTaskDto dto)
    {
        return new Core.Entities.Task
        {
            Title = dto.Title,
            Description = dto.Description,
            Status = dto.Status,
            Priority = dto.Priority,
            StartDate = dto.StartDate,
            DueDate = dto.DueDate,
            ProjectId = dto.ProjectId,
            ParentTaskId = dto.ParentTaskId
        };
    }

    /// <summary>
    /// Updates an existing Task entity with data from UpdateTaskDto
    /// All fields except ProjectId can be modified
    /// </summary>
    public static void UpdateFromDto(this Core.Entities.Task task, UpdateTaskDto dto)
    {
        task.Title = dto.Title;
        task.Description = dto.Description;
        task.Status = dto.Status;
        task.Priority = dto.Priority;
        task.StartDate = dto.StartDate;
        task.DueDate = dto.DueDate;
    }

    /// <summary>
    /// Updates only the Status field from UpdateTaskStatusDto
    /// Used for atomic status updates without affecting other fields
    /// </summary>
    public static void UpdateStatusFromDto(this Core.Entities.Task task, UpdateTaskStatusDto dto)
    {
        task.Status = dto.NewStatus;
    }

    /// <summary>
    /// Updates only the Priority field from UpdateTaskPriorityDto
    /// Used for atomic priority updates without affecting other fields
    /// </summary>
    public static void UpdatePriorityFromDto(this Core.Entities.Task task, UpdateTaskPriorityDto dto)
    {
        task.Priority = dto.NewPriority;
    }

    /// <summary>
    /// Converts a collection of Task entities to TaskDto list
    /// </summary>
    public static List<TaskDto> ToDtoList(this IEnumerable<Core.Entities.Task> tasks)
    {
        return tasks.Select(t => t.ToDto()).ToList();
    }

    /// <summary>
    /// Converts TaskAssignment entity to TaskAssignmentDto with denormalized user data
    /// Includes user information directly in the DTO
    /// </summary>
    public static TaskAssignmentDto ToAssignmentDto(this TaskAssignment assignment)
    {
        return new TaskAssignmentDto
        {
            TaskId = assignment.TaskId,
            TaskTitle = assignment.Task?.Title ?? "Unknown",
            UserId = assignment.UserId,
            Username = assignment.User?.Username ?? "Unknown",
            Email = assignment.User?.Email ?? "Unknown",
            AssignedAt = assignment.CreatedAt
        };
    }

    /// <summary>
    /// Converts a collection of TaskAssignment entities to TaskAssignmentDto list
    /// </summary>
    public static List<TaskAssignmentDto> ToAssignmentDtoList(this IEnumerable<TaskAssignment> assignments)
    {
        return assignments.Select(a => a.ToAssignmentDto()).ToList();
    }
}
