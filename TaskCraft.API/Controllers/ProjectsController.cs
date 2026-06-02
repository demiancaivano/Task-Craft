using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskCraft.Application.DTOs.Project;
using TaskCraft.Application.Interfaces;
using TaskCraft.Application.Mappings;
using TaskCraft.Core.Enums;
using TaskCraft.Core.Exceptions;
using TaskCraft.Core.Interfaces;

namespace TaskCraft.API.Controllers;

/// <summary>
/// Controller for project management operations
/// Handles project CRUD and member management
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProjectsController : ControllerBase
{
    private readonly IProjectService _projectService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ProjectsController> _logger;

    public ProjectsController(
        IProjectService projectService,
        IUnitOfWork unitOfWork,
        ILogger<ProjectsController> logger)
    {
        _projectService = projectService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    private Guid GetCurrentUserId()
    {
        var sub = User.FindFirstValue(ClaimTypes.NameIdentifier)
               ?? User.FindFirstValue("sub");
        return Guid.Parse(sub!);
    }

    private async Task<ProjectRole?> GetCallerRoleAsync(Guid projectId)
    {
        var userId = GetCurrentUserId();
        return await _projectService.GetUserRoleInProjectAsync(userId, projectId);
    }

    /// <summary>
    /// Get all projects (optionally include soft-deleted)
    /// </summary>
    /// <param name="includeDeleted">Whether to include soft-deleted projects</param>
    /// <returns>List of projects</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ProjectDto>))]
    public async Task<ActionResult<IEnumerable<ProjectDto>>> GetAllProjects()
    {
        try
        {
            var userId = GetCurrentUserId();
            var projects = await _unitOfWork.Projects.GetByUserIdAsync(userId);
            var projectDtos = projects.ToDtoList(userId);

            _logger.LogInformation("Retrieved {Count} projects for user {UserId}",
                projectDtos.Count, userId);

            return Ok(projectDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving projects");
            return StatusCode(500, new { message = "An error occurred while retrieving projects" });
        }
    }

    /// <summary>
    /// Get a specific project by ID
    /// </summary>
    /// <param name="id">Project ID</param>
    /// <returns>Project details</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProjectDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProjectDto>> GetProjectById(Guid id)
    {
        try
        {
            var project = await _projectService.GetProjectByIdAsync(id);

            if (project == null)
            {
                _logger.LogWarning("Project with ID {ProjectId} not found", id);
                return NotFound(new { message = $"Project with ID {id} not found" });
            }

            var userId = GetCurrentUserId();
            var projectDto = project.ToDto(userId);
            _logger.LogInformation("Retrieved project {ProjectId}", id);

            return Ok(projectDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving project {ProjectId}", id);
            return StatusCode(500, new { message = "An error occurred while retrieving the project" });
        }
    }

    /// <summary>
    /// Get all projects for a specific user (as owner or member)
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>List of projects</returns>
    [HttpGet("user/{userId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ProjectDto>))]
    public async Task<ActionResult<IEnumerable<ProjectDto>>> GetProjectsByUserId(Guid userId)
    {
        try
        {
            var projects = await _unitOfWork.Projects.GetByUserIdAsync(userId);
            var projectDtos = projects.ToDtoList();

            _logger.LogInformation("Retrieved {Count} projects for user {UserId}", 
                projectDtos.Count, userId);

            return Ok(projectDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving projects for user {UserId}", userId);
            return StatusCode(500, new { message = "An error occurred while retrieving projects" });
        }
    }

    /// <summary>
    /// Create a new project
    /// Owner is automatically added as Manager role
    /// </summary>
    /// <param name="createProjectDto">Project creation data</param>
    /// <returns>Created project details</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ProjectDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProjectDto>> CreateProject([FromBody] CreateProjectDto createProjectDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Override ownerId with authenticated user
            createProjectDto.OwnerId = GetCurrentUserId();

            var createdProject = await _projectService.CreateProjectAsync(createProjectDto);
            await _unitOfWork.SaveChangesAsync();

            var projectDto = createdProject.ToDto();
            _logger.LogInformation("Created new project {ProjectId} with name {ProjectName}", 
                projectDto.Id, projectDto.Name);

            return CreatedAtAction(
                nameof(GetProjectById),
                new { id = projectDto.Id },
                projectDto);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Owner not found: {Message}", ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Project creation failed: {Message}", ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating project");
            return StatusCode(500, new { message = "An error occurred while creating the project" });
        }
    }

    /// <summary>
    /// Update an existing project (name and description only)
    /// </summary>
    /// <param name="id">Project ID</param>
    /// <param name="updateProjectDto">Updated project data</param>
    /// <returns>Updated project details</returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProjectDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProjectDto>> UpdateProject(Guid id, [FromBody] UpdateProjectDto updateProjectDto)
    {
        try
        {
            if (id != updateProjectDto.Id)
                return BadRequest(new { message = "ID in URL does not match ID in request body" });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var role = await GetCallerRoleAsync(id);
            if (role != ProjectRole.Manager)
                return Forbid();

            var updatedProject = await _projectService.UpdateProjectAsync(updateProjectDto);
            await _unitOfWork.SaveChangesAsync();

            var projectDto = updatedProject.ToDto();
            _logger.LogInformation("Updated project {ProjectId}", id);

            return Ok(projectDto);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Project {ProjectId} not found for update", id);
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Project update failed: {Message}", ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating project {ProjectId}", id);
            return StatusCode(500, new { message = "An error occurred while updating the project" });
        }
    }

    /// <summary>
    /// Soft delete a project
    /// </summary>
    /// <param name="id">Project ID</param>
    /// <returns>No content on success</returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteProject(Guid id)
    {
        try
        {
            var role = await GetCallerRoleAsync(id);
            if (role != ProjectRole.Manager)
                return Forbid();

            await _projectService.DeleteProjectAsync(id);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Soft deleted project {ProjectId}", id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Project {ProjectId} not found for deletion", id);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting project {ProjectId}", id);
            return StatusCode(500, new { message = "An error occurred while deleting the project" });
        }
    }

    /// <summary>
    /// Get all members of a project
    /// </summary>
    /// <param name="id">Project ID</param>
    /// <returns>List of project members</returns>
    [HttpGet("{id:guid}/members")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ProjectMemberDto>))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<ProjectMemberDto>>> GetProjectMembers(Guid id)
    {
        try
        {
            var members = await _unitOfWork.ProjectMembers.GetByProjectIdAsync(id);
            var memberDtos = members.ToMemberDtoList();

            _logger.LogInformation("Retrieved {Count} members for project {ProjectId}", 
                memberDtos.Count, id);

            return Ok(memberDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving members for project {ProjectId}", id);
            return StatusCode(500, new { message = "An error occurred while retrieving project members" });
        }
    }

    /// <summary>
    /// Add a member to a project with a specific role
    /// </summary>
    /// <param name="id">Project ID</param>
    /// <param name="addMemberDto">Member addition data</param>
    /// <returns>Created project member details</returns>
    [HttpPost("{id:guid}/members")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ProjectMemberDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProjectMemberDto>> AddProjectMember(
        Guid id,
        [FromBody] AddProjectMemberDto addMemberDto)
    {
        try
        {
            // Always use the project ID from the route
            addMemberDto.ProjectId = id;

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var role = await GetCallerRoleAsync(id);
            if (role != ProjectRole.Manager)
                return Forbid();

            var member = await _projectService.AddMemberToProjectAsync(addMemberDto);
            await _unitOfWork.SaveChangesAsync();

            var memberDto = member.ToMemberDto();
            _logger.LogInformation("Added user {UserId} to project {ProjectId} with role {Role}", 
                addMemberDto.UserId, id, addMemberDto.Role);

            return CreatedAtAction(
                nameof(GetProjectMembers),
                new { id = id },
                memberDto);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Project or user not found: {Message}", ex.Message);
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Failed to add member: {Message}", ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding member to project {ProjectId}", id);
            return StatusCode(500, new { message = "An error occurred while adding the member" });
        }
    }

    /// <summary>
    /// Remove a member from a project (soft delete)
    /// Cannot remove the project owner
    /// </summary>
    /// <param name="projectId">Project ID</param>
    /// <param name="userId">User ID to remove</param>
    /// <returns>No content on success</returns>
    [HttpDelete("{projectId:guid}/members/{userId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveProjectMember(Guid projectId, Guid userId)
    {
        try
        {
            var role = await GetCallerRoleAsync(projectId);
            if (role != ProjectRole.Manager)
                return Forbid();

            await _projectService.RemoveMemberFromProjectAsync(projectId, userId);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Removed user {UserId} from project {ProjectId}", userId, projectId);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Project or member not found: {Message}", ex.Message);
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Failed to remove member: {Message}", ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing member from project {ProjectId}", projectId);
            return StatusCode(500, new { message = "An error occurred while removing the member" });
        }
    }

    /// <summary>
    /// Leave a project as the current authenticated user.
    /// Managers (project owners) cannot leave their own project.
    /// </summary>
    /// <param name="projectId">Project ID</param>
    /// <returns>No content on success</returns>
    [HttpDelete("{projectId:guid}/leave")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> LeaveProject(Guid projectId)
    {
        try
        {
            var currentUserId = GetCurrentUserId();

            await _projectService.RemoveMemberFromProjectAsync(projectId, currentUserId);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("User {UserId} left project {ProjectId}", currentUserId, projectId);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Project or membership not found while user attempted to leave: {Message}", ex.Message);
            return NotFound(new { message = ex.Message });
        }
        catch (BadRequestException ex)
        {
            _logger.LogWarning(ex, "User failed to leave project: {Message}", ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while user leaving project {ProjectId}", projectId);
            return StatusCode(500, new { message = "An error occurred while leaving the project" });
        }
    }
}
