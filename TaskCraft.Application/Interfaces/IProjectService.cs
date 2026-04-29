using TaskCraft.Application.DTOs.Project;
using TaskCraft.Core.Entities;
using TaskCraft.Core.Enums;

namespace TaskCraft.Application.Interfaces;

/// <summary>
/// Service interface for project-related business operations
/// </summary>
public interface IProjectService
{
    Task<IEnumerable<Project>> GetAllProjectsAsync(bool includeDeleted = false, CancellationToken cancellationToken = default);
    Task<Project?> GetProjectByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Project?> GetProjectWithMembersAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Project?> GetProjectWithTasksAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Project>> GetProjectsByOwnerIdAsync(Guid ownerId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Project>> GetProjectsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Project> CreateProjectAsync(Project project, CancellationToken cancellationToken = default);
    Task<Project> CreateProjectAsync(CreateProjectDto createProjectDto, CancellationToken cancellationToken = default);
    Task<Project> UpdateProjectAsync(Project project, CancellationToken cancellationToken = default);
    Task<Project> UpdateProjectAsync(UpdateProjectDto updateProjectDto, CancellationToken cancellationToken = default);
    Task<bool> DeleteProjectAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ProjectMember> AddMemberToProjectAsync(Guid projectId, Guid userId, ProjectRole role, CancellationToken cancellationToken = default);
    Task<ProjectMember> AddMemberToProjectAsync(AddProjectMemberDto addMemberDto, CancellationToken cancellationToken = default);
    Task<bool> RemoveMemberFromProjectAsync(Guid projectId, Guid userId, CancellationToken cancellationToken = default);
    Task<bool> UpdateMemberRoleAsync(Guid projectId, Guid userId, ProjectRole newRole, CancellationToken cancellationToken = default);
    Task<bool> IsUserMemberOfProjectAsync(Guid userId, Guid projectId, CancellationToken cancellationToken = default);
    Task<ProjectRole?> GetUserRoleInProjectAsync(Guid userId, Guid projectId, CancellationToken cancellationToken = default);
}
