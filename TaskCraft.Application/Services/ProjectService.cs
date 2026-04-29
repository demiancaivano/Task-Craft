using TaskCraft.Application.DTOs.Project;
using TaskCraft.Application.Interfaces;
using TaskCraft.Application.Mappings;
using TaskCraft.Core.Entities;
using TaskCraft.Core.Enums;
using TaskCraft.Core.Exceptions;
using TaskCraft.Core.Interfaces;

namespace TaskCraft.Application.Services;

/// <summary>
/// Service implementation for project-related business operations
/// </summary>
public class ProjectService : IProjectService
{
    private readonly IUnitOfWork _unitOfWork;

    public ProjectService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<Project>> GetAllProjectsAsync(bool includeDeleted = false, CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.Projects.GetAllAsync(includeDeleted, cancellationToken);
    }

    public async Task<Project?> GetProjectByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.Projects.GetByIdAsync(id, cancellationToken);
    }

    public async Task<Project?> GetProjectWithMembersAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.Projects.GetProjectWithMembersAsync(id, cancellationToken);
    }

    public async Task<Project?> GetProjectWithTasksAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.Projects.GetProjectWithTasksAsync(id, cancellationToken);
    }

    public async Task<IEnumerable<Project>> GetProjectsByOwnerIdAsync(Guid ownerId, CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.Projects.GetProjectsByOwnerIdAsync(ownerId, cancellationToken);
    }

    public async Task<IEnumerable<Project>> GetProjectsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.Projects.GetProjectsByUserIdAsync(userId, cancellationToken);
    }

    public async Task<Project> CreateProjectAsync(Project project, CancellationToken cancellationToken = default)
    {
        // Business validation
        var validationErrors = new Dictionary<string, string[]>();

        if (string.IsNullOrWhiteSpace(project.Name))
            validationErrors.Add("Name", new[] { "Project name is required" });

        if (project.OwnerId == Guid.Empty)
            validationErrors.Add("OwnerId", new[] { "Project owner is required" });

        if (validationErrors.Any())
            throw new ValidationException(validationErrors);

        // Verify owner exists
        var owner = await _unitOfWork.Users.GetByIdAsync(project.OwnerId, cancellationToken);
        if (owner == null)
            throw new NotFoundException("User", project.OwnerId);

        // Create project
        await _unitOfWork.Projects.AddAsync(project, cancellationToken);

        // Automatically add owner as Manager
        var ownerMembership = new ProjectMember
        {
            ProjectId = project.Id,
            UserId = project.OwnerId,
            Role = ProjectRole.Manager,
            JoinedAt = DateTime.UtcNow
        };
        await _unitOfWork.ProjectMembers.AddAsync(ownerMembership, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return project;
    }

    public async Task<Project> CreateProjectAsync(CreateProjectDto createProjectDto, CancellationToken cancellationToken = default)
    {
        var project = createProjectDto.ToEntity();
        return await CreateProjectAsync(project, cancellationToken);
    }

    public async Task<Project> UpdateProjectAsync(Project project, CancellationToken cancellationToken = default)
    {
        // Check if project exists
        var existingProject = await _unitOfWork.Projects.GetByIdAsync(project.Id, cancellationToken);
        if (existingProject == null)
            throw new NotFoundException("Project", project.Id);

        // Business validation
        if (string.IsNullOrWhiteSpace(project.Name))
        {
            var errors = new Dictionary<string, string[]>
            {
                { "Name", new[] { "Project name is required" } }
            };
            throw new ValidationException(errors);
        }

        // Update project
        await _unitOfWork.Projects.UpdateAsync(project, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return project;
    }

    public async Task<Project> UpdateProjectAsync(UpdateProjectDto updateProjectDto, CancellationToken cancellationToken = default)
    {
        var existingProject = await _unitOfWork.Projects.GetByIdAsync(updateProjectDto.Id, cancellationToken);
        if (existingProject == null)
            throw new NotFoundException("Project", updateProjectDto.Id);

        existingProject.Name = updateProjectDto.Name;
        existingProject.Description = updateProjectDto.Description;

        return await UpdateProjectAsync(existingProject, cancellationToken);
    }

    public async Task<bool> DeleteProjectAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var project = await _unitOfWork.Projects.GetByIdAsync(id, cancellationToken);
        if (project == null)
            throw new NotFoundException("Project", id);

        // Soft delete
        await _unitOfWork.Projects.SoftDeleteAsync(id, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<ProjectMember> AddMemberToProjectAsync(Guid projectId, Guid userId, ProjectRole role, CancellationToken cancellationToken = default)
    {
        // Verify project exists
        var project = await _unitOfWork.Projects.GetByIdAsync(projectId, cancellationToken);
        if (project == null)
            throw new NotFoundException("Project", projectId);

        // Verify user exists
        var user = await _unitOfWork.Users.GetByIdAsync(userId, cancellationToken);
        if (user == null)
            throw new NotFoundException("User", userId);

        // Check if user is already a member
        var existingMembership = await _unitOfWork.ProjectMembers.GetByUserAndProjectAsync(userId, projectId, cancellationToken);
        if (existingMembership != null)
            throw new ConflictException("User is already a member of this project");

        // Add member
        var member = new ProjectMember
        {
            ProjectId = projectId,
            UserId = userId,
            Role = role,
            JoinedAt = DateTime.UtcNow
        };

        await _unitOfWork.ProjectMembers.AddAsync(member, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return member;
    }

    public async Task<ProjectMember> AddMemberToProjectAsync(AddProjectMemberDto addMemberDto, CancellationToken cancellationToken = default)
    {
        return await AddMemberToProjectAsync(addMemberDto.ProjectId, addMemberDto.UserId, addMemberDto.Role, cancellationToken);
    }

    public async Task<bool> RemoveMemberFromProjectAsync(Guid projectId, Guid userId, CancellationToken cancellationToken = default)
    {
        // Get membership
        var membership = await _unitOfWork.ProjectMembers.GetByUserAndProjectAsync(userId, projectId, cancellationToken);
        if (membership == null)
            throw new NotFoundException("ProjectMember", $"User {userId} in Project {projectId}");

        // Prevent removing the project owner
        var project = await _unitOfWork.Projects.GetByIdAsync(projectId, cancellationToken);
        if (project != null && project.OwnerId == userId)
            throw new BadRequestException("Cannot remove the project owner from the project");

        // Remove member
        await _unitOfWork.ProjectMembers.DeleteAsync(membership.Id, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> UpdateMemberRoleAsync(Guid projectId, Guid userId, ProjectRole newRole, CancellationToken cancellationToken = default)
    {
        // Get membership
        var membership = await _unitOfWork.ProjectMembers.GetByUserAndProjectAsync(userId, projectId, cancellationToken);
        if (membership == null)
            throw new NotFoundException("ProjectMember", $"User {userId} in Project {projectId}");

        // Update role
        membership.Role = newRole;
        await _unitOfWork.ProjectMembers.UpdateAsync(membership, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> IsUserMemberOfProjectAsync(Guid userId, Guid projectId, CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.ProjectMembers.IsUserMemberOfProjectAsync(userId, projectId, cancellationToken);
    }

    public async Task<ProjectRole?> GetUserRoleInProjectAsync(Guid userId, Guid projectId, CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.ProjectMembers.GetUserRoleInProjectAsync(userId, projectId, cancellationToken);
    }
}
