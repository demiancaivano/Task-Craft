using TaskCraft.Application.DTOs.Project;
using TaskCraft.Core.Entities;

namespace TaskCraft.Application.Mappings;

/// <summary>
/// Extension methods for mapping between Project entity and Project DTOs
/// </summary>
public static class ProjectMappingExtensions
{
    /// <summary>
    /// Converts Project entity to ProjectDto
    /// Calculates MemberCount and TaskCount from collections
    /// </summary>
    public static ProjectDto ToDto(this Project project)
    {
        return new ProjectDto
        {
            Id = project.Id,
            Name = project.Name,
            Description = project.Description,
            OwnerId = project.OwnerId,
            OwnerUsername = project.Owner?.Username ?? "Unknown",
            CreatedAt = project.CreatedAt,
            MemberCount = project.Members?.Count(m => !m.IsDeleted) ?? 0,
            TaskCount = project.Tasks?.Count(t => !t.IsDeleted) ?? 0
        };
    }

    /// <summary>
    /// Converts CreateProjectDto to Project entity
    /// Owner should be added as Manager through ProjectService after creation
    /// </summary>
    public static Project ToEntity(this CreateProjectDto dto)
    {
        return new Project
        {
            Name = dto.Name,
            Description = dto.Description,
            OwnerId = dto.OwnerId
        };
    }

    /// <summary>
    /// Updates an existing Project entity with data from UpdateProjectDto
    /// Only updates Name and Description (Owner cannot be changed)
    /// </summary>
    public static void UpdateFromDto(this Project project, UpdateProjectDto dto)
    {
        project.Name = dto.Name;
        project.Description = dto.Description;
    }

    /// <summary>
    /// Converts a collection of Project entities to ProjectDto list
    /// </summary>
    public static List<ProjectDto> ToDtoList(this IEnumerable<Project> projects)
    {
        return projects.Select(p => p.ToDto()).ToList();
    }

    /// <summary>
    /// Converts ProjectMember entity to ProjectMemberDto with denormalized user data
    /// Includes user information directly in the DTO for convenience
    /// </summary>
    public static ProjectMemberDto ToMemberDto(this ProjectMember member)
    {
        return new ProjectMemberDto
        {
            Id = member.Id,
            UserId = member.UserId,
            Username = member.User?.Username ?? "Unknown",
            Email = member.User?.Email ?? "Unknown",
            ProjectId = member.ProjectId,
            Role = member.Role,
            JoinedAt = member.CreatedAt
        };
    }

    /// <summary>
    /// Converts a collection of ProjectMember entities to ProjectMemberDto list
    /// </summary>
    public static List<ProjectMemberDto> ToMemberDtoList(this IEnumerable<ProjectMember> members)
    {
        return members.Select(m => m.ToMemberDto()).ToList();
    }
}
