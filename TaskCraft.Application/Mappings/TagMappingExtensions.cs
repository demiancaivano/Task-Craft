using TaskCraft.Application.DTOs.Tag;
using TaskCraft.Core.Entities;

namespace TaskCraft.Application.Mappings;

/// <summary>
/// Extension methods for mapping between Tag entity and Tag DTOs
/// </summary>
public static class TagMappingExtensions
{
    /// <summary>
    /// Converts Tag entity to TagDto
    /// Calculates UsageCount by counting associated TaskTags
    /// </summary>
    public static TagDto ToDto(this Tag tag)
    {
        return new TagDto
        {
            Id = tag.Id,
            Name = tag.Name,
            Color = tag.Color,
            ProjectId = tag.ProjectId,
            UsageCount = tag.TaskTags?.Count(tt => !tt.IsDeleted && !tt.Task.IsDeleted) ?? 0
        };
    }

    /// <summary>
    /// Converts CreateTagDto to Tag entity
    /// Color format validation should be done in service layer or DTO validation
    /// </summary>
    public static Tag ToEntity(this CreateTagDto dto)
    {
        return new Tag
        {
            Name = dto.Name,
            Color = dto.Color,
            ProjectId = dto.ProjectId
        };
    }

    /// <summary>
    /// Converts a collection of Tag entities to TagDto list
    /// </summary>
    public static List<TagDto> ToDtoList(this IEnumerable<Tag> tags)
    {
        return tags.Select(t => t.ToDto()).ToList();
    }
}
