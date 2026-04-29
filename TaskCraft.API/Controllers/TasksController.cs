using Microsoft.AspNetCore.Mvc;
using TaskCraft.Application.DTOs.Task;
using TaskCraft.Application.Interfaces;
using TaskCraft.Application.Mappings;
using TaskCraft.Core.Enums;
using TaskCraft.Core.Interfaces;
using TaskStatus = TaskCraft.Core.Enums.TaskStatus;

namespace TaskCraft.API.Controllers;

/// <summary>
/// Controller for task management operations
/// Handles task CRUD, status/priority updates, and assignments
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly ITaskService _taskService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<TasksController> _logger;

    public TasksController(
        ITaskService taskService,
        IUnitOfWork unitOfWork,
        ILogger<TasksController> logger)
    {
        _taskService = taskService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    /// <summary>
    /// Get all tasks (optionally include soft-deleted)
    /// </summary>
    /// <param name="includeDeleted">Whether to include soft-deleted tasks</param>
    /// <returns>List of tasks</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<TaskDto>))]
    public async Task<ActionResult<IEnumerable<TaskDto>>> GetAllTasks([FromQuery] bool includeDeleted = false)
    {
        try
        {
            var tasks = await _taskService.GetAllTasksAsync(includeDeleted);
            var taskDtos = tasks.ToDtoList();

            _logger.LogInformation("Retrieved {Count} tasks (includeDeleted: {IncludeDeleted})", 
                taskDtos.Count, includeDeleted);

            return Ok(taskDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving tasks");
            return StatusCode(500, new { message = "An error occurred while retrieving tasks" });
        }
    }

    /// <summary>
    /// Get a specific task by ID
    /// </summary>
    /// <param name="id">Task ID</param>
    /// <returns>Task details</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TaskDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TaskDto>> GetTaskById(Guid id)
    {
        try
        {
            var task = await _taskService.GetTaskByIdAsync(id);

            if (task == null)
            {
                _logger.LogWarning("Task with ID {TaskId} not found", id);
                return NotFound(new { message = $"Task with ID {id} not found" });
            }

            var taskDto = task.ToDto();
            _logger.LogInformation("Retrieved task {TaskId}", id);

            return Ok(taskDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving task {TaskId}", id);
            return StatusCode(500, new { message = "An error occurred while retrieving the task" });
        }
    }

    /// <summary>
    /// Get all tasks for a specific project
    /// </summary>
    /// <param name="projectId">Project ID</param>
    /// <returns>List of tasks</returns>
    [HttpGet("project/{projectId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<TaskDto>))]
    public async Task<ActionResult<IEnumerable<TaskDto>>> GetTasksByProjectId(Guid projectId)
    {
        try
        {
            var tasks = await _unitOfWork.Tasks.GetByProjectIdAsync(projectId);
            var taskDtos = tasks.ToDtoList();

            _logger.LogInformation("Retrieved {Count} tasks for project {ProjectId}", 
                taskDtos.Count, projectId);

            return Ok(taskDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving tasks for project {ProjectId}", projectId);
            return StatusCode(500, new { message = "An error occurred while retrieving tasks" });
        }
    }

    /// <summary>
    /// Get all tasks assigned to a specific user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>List of tasks</returns>
    [HttpGet("user/{userId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<TaskDto>))]
    public async Task<ActionResult<IEnumerable<TaskDto>>> GetTasksByUserId(Guid userId)
    {
        try
        {
            var tasks = await _unitOfWork.Tasks.GetByUserIdAsync(userId);
            var taskDtos = tasks.ToDtoList();

            _logger.LogInformation("Retrieved {Count} tasks assigned to user {UserId}", 
                taskDtos.Count, userId);

            return Ok(taskDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving tasks for user {UserId}", userId);
            return StatusCode(500, new { message = "An error occurred while retrieving tasks" });
        }
    }

    /// <summary>
    /// Get all tasks with a specific status
    /// </summary>
    /// <param name="status">Task status</param>
    /// <returns>List of tasks</returns>
    [HttpGet("status/{status}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<TaskDto>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<TaskDto>>> GetTasksByStatus(TaskStatus status)
    {
        try
        {
            var tasks = await _unitOfWork.Tasks.GetByStatusAsync(status);
            var taskDtos = tasks.ToDtoList();

            _logger.LogInformation("Retrieved {Count} tasks with status {Status}", 
                taskDtos.Count, status);

            return Ok(taskDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving tasks by status {Status}", status);
            return StatusCode(500, new { message = "An error occurred while retrieving tasks" });
        }
    }

    /// <summary>
    /// Get all overdue tasks (DueDate passed and status not Done/Blocked)
    /// </summary>
    /// <returns>List of overdue tasks</returns>
    [HttpGet("overdue")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<TaskDto>))]
    public async Task<ActionResult<IEnumerable<TaskDto>>> GetOverdueTasks()
    {
        try
        {
            var tasks = await _unitOfWork.Tasks.GetOverdueTasksAsync();
            var taskDtos = tasks.ToDtoList();

            _logger.LogInformation("Retrieved {Count} overdue tasks", taskDtos.Count);

            return Ok(taskDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving overdue tasks");
            return StatusCode(500, new { message = "An error occurred while retrieving overdue tasks" });
        }
    }

    /// <summary>
    /// Get all subtasks of a parent task
    /// </summary>
    /// <param name="parentTaskId">Parent task ID</param>
    /// <returns>List of subtasks</returns>
    [HttpGet("{parentTaskId:guid}/subtasks")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<TaskDto>))]
    public async Task<ActionResult<IEnumerable<TaskDto>>> GetSubTasks(Guid parentTaskId)
    {
        try
        {
            var subtasks = await _unitOfWork.Tasks.GetSubTasksAsync(parentTaskId);
            var taskDtos = subtasks.ToDtoList();

            _logger.LogInformation("Retrieved {Count} subtasks for task {ParentTaskId}", 
                taskDtos.Count, parentTaskId);

            return Ok(taskDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving subtasks for task {ParentTaskId}", parentTaskId);
            return StatusCode(500, new { message = "An error occurred while retrieving subtasks" });
        }
    }

    /// <summary>
    /// Create a new task
    /// Validates subtask depth and project membership
    /// </summary>
    /// <param name="createTaskDto">Task creation data</param>
    /// <returns>Created task details</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(TaskDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TaskDto>> CreateTask([FromBody] CreateTaskDto createTaskDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdTask = await _taskService.CreateTaskAsync(createTaskDto);
            await _unitOfWork.SaveChangesAsync();

            var taskDto = createdTask.ToDto();
            _logger.LogInformation("Created new task {TaskId} with title {TaskTitle}", 
                taskDto.Id, taskDto.Title);

            return CreatedAtAction(
                nameof(GetTaskById),
                new { id = taskDto.Id },
                taskDto);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Project or parent task not found: {Message}", ex.Message);
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Task creation failed: {Message}", ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating task");
            return StatusCode(500, new { message = "An error occurred while creating the task" });
        }
    }

    /// <summary>
    /// Update an existing task (full update)
    /// </summary>
    /// <param name="id">Task ID</param>
    /// <param name="updateTaskDto">Updated task data</param>
    /// <returns>Updated task details</returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TaskDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TaskDto>> UpdateTask(Guid id, [FromBody] UpdateTaskDto updateTaskDto)
    {
        try
        {
            if (id != updateTaskDto.Id)
            {
                return BadRequest(new { message = "ID in URL does not match ID in request body" });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updatedTask = await _taskService.UpdateTaskAsync(updateTaskDto);
            await _unitOfWork.SaveChangesAsync();

            var taskDto = updatedTask.ToDto();
            _logger.LogInformation("Updated task {TaskId}", id);

            return Ok(taskDto);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Task {TaskId} not found for update", id);
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Task update failed: {Message}", ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating task {TaskId}", id);
            return StatusCode(500, new { message = "An error occurred while updating the task" });
        }
    }

    /// <summary>
    /// Update task status only (atomic operation)
    /// Used for quick status changes without loading full entity
    /// </summary>
    /// <param name="id">Task ID</param>
    /// <param name="updateStatusDto">Status update data</param>
    /// <returns>Updated task details</returns>
    [HttpPatch("{id:guid}/status")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TaskDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TaskDto>> UpdateTaskStatus(Guid id, [FromBody] UpdateTaskStatusDto updateStatusDto)
    {
        try
        {
            if (id != updateStatusDto.TaskId)
            {
                return BadRequest(new { message = "ID in URL does not match TaskId in request body" });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var task = await _unitOfWork.Tasks.GetByIdAsync(id);
            if (task == null)
            {
                return NotFound(new { message = $"Task with ID {id} not found" });
            }

            task.UpdateStatusFromDto(updateStatusDto);
            await _unitOfWork.SaveChangesAsync();

            var taskDto = task.ToDto();
            _logger.LogInformation("Updated status of task {TaskId} to {NewStatus}", id, updateStatusDto.NewStatus);

            return Ok(taskDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating task status for task {TaskId}", id);
            return StatusCode(500, new { message = "An error occurred while updating task status" });
        }
    }

    /// <summary>
    /// Update task priority only (atomic operation)
    /// Used for quick priority changes without loading full entity
    /// </summary>
    /// <param name="id">Task ID</param>
    /// <param name="updatePriorityDto">Priority update data</param>
    /// <returns>Updated task details</returns>
    [HttpPatch("{id:guid}/priority")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TaskDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TaskDto>> UpdateTaskPriority(Guid id, [FromBody] UpdateTaskPriorityDto updatePriorityDto)
    {
        try
        {
            if (id != updatePriorityDto.TaskId)
            {
                return BadRequest(new { message = "ID in URL does not match TaskId in request body" });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var task = await _unitOfWork.Tasks.GetByIdAsync(id);
            if (task == null)
            {
                return NotFound(new { message = $"Task with ID {id} not found" });
            }

            task.UpdatePriorityFromDto(updatePriorityDto);
            await _unitOfWork.SaveChangesAsync();

            var taskDto = task.ToDto();
            _logger.LogInformation("Updated priority of task {TaskId} to {NewPriority}", id, updatePriorityDto.NewPriority);

            return Ok(taskDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating task priority for task {TaskId}", id);
            return StatusCode(500, new { message = "An error occurred while updating task priority" });
        }
    }

    /// <summary>
    /// Soft delete a task
    /// Cannot delete if task has active subtasks
    /// </summary>
    /// <param name="id">Task ID</param>
    /// <returns>No content on success</returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteTask(Guid id)
    {
        try
        {
            await _taskService.DeleteTaskAsync(id);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Soft deleted task {TaskId}", id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Task {TaskId} not found for deletion", id);
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Task deletion failed: {Message}", ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting task {TaskId}", id);
            return StatusCode(500, new { message = "An error occurred while deleting the task" });
        }
    }

    /// <summary>
    /// Assign a user to a task
    /// Validates that user is a member of the task's project
    /// </summary>
    /// <param name="id">Task ID</param>
    /// <param name="assignTaskDto">Assignment data</param>
    /// <returns>Created assignment details</returns>
    [HttpPost("{id:guid}/assign")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(TaskAssignmentDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TaskAssignmentDto>> AssignUserToTask(
        Guid id, 
        [FromBody] AssignTaskDto assignTaskDto)
    {
        try
        {
            if (id != assignTaskDto.TaskId)
            {
                return BadRequest(new { message = "ID in URL does not match TaskId in request body" });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var assignment = await _taskService.AssignUserToTaskAsync(assignTaskDto);
            await _unitOfWork.SaveChangesAsync();

            var assignmentDto = assignment.ToAssignmentDto();
            _logger.LogInformation("Assigned user {UserId} to task {TaskId}", 
                assignTaskDto.UserId, id);

            return CreatedAtAction(
                nameof(GetTaskAssignments),
                new { id = id },
                assignmentDto);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Task or user not found: {Message}", ex.Message);
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Assignment failed: {Message}", ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning user to task {TaskId}", id);
            return StatusCode(500, new { message = "An error occurred while assigning the user" });
        }
    }

    /// <summary>
    /// Unassign a user from a task (soft delete)
    /// </summary>
    /// <param name="taskId">Task ID</param>
    /// <param name="userId">User ID to unassign</param>
    /// <returns>No content on success</returns>
    [HttpDelete("{taskId:guid}/assign/{userId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UnassignUserFromTask(Guid taskId, Guid userId)
    {
        try
        {
            await _taskService.UnassignUserFromTaskAsync(taskId, userId);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Unassigned user {UserId} from task {TaskId}", userId, taskId);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Assignment not found: {Message}", ex.Message);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error unassigning user from task {TaskId}", taskId);
            return StatusCode(500, new { message = "An error occurred while unassigning the user" });
        }
    }

    /// <summary>
    /// Get all assignments for a specific task
    /// </summary>
    /// <param name="id">Task ID</param>
    /// <returns>List of task assignments</returns>
    [HttpGet("{id:guid}/assignments")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<TaskAssignmentDto>))]
    public async Task<ActionResult<IEnumerable<TaskAssignmentDto>>> GetTaskAssignments(Guid id)
    {
        try
        {
            var assignments = await _unitOfWork.TaskAssignments.GetByTaskIdAsync(id);
            var assignmentDtos = assignments.ToAssignmentDtoList();

            _logger.LogInformation("Retrieved {Count} assignments for task {TaskId}", 
                assignmentDtos.Count, id);

            return Ok(assignmentDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving assignments for task {TaskId}", id);
            return StatusCode(500, new { message = "An error occurred while retrieving assignments" });
        }
    }
}
