namespace TaskCraft.Core.Enums;

/// <summary>
/// Roles that users can have within a project
/// </summary>
public enum ProjectRole
{
    /// <summary>
    /// Project manager with full permissions
    /// </summary>
    Manager = 1,

    /// <summary>
    /// Developer who can create and modify tasks
    /// </summary>
    Developer = 2,

    /// <summary>
    /// Regular member who can view and comment
    /// </summary>
    Member = 3,

    /// <summary>
    /// Viewer with read-only access
    /// </summary>
    Viewer = 4
}
