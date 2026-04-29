using FluentAssertions;
using TaskCraft.Application.DTOs.Task;
using TaskCraft.Application.Validators.Task;
using TaskCraft.Core.Enums;
using Xunit;
using TaskStatus = TaskCraft.Core.Enums.TaskStatus;

namespace TaskCraft.Tests.Validators.Task;

/// <summary>
/// Unit tests for CreateTaskDtoValidator
/// </summary>
public class CreateTaskDtoValidatorTests
{
    private readonly CreateTaskDtoValidator _validator;

    public CreateTaskDtoValidatorTests()
    {
        _validator = new CreateTaskDtoValidator();
    }

    #region Valid Cases

    [Fact]
    public void Should_Pass_When_All_Required_Fields_Are_Valid()
    {
        // Arrange
        var dto = new CreateTaskDto
        {
            Title = "Valid Task Title",
            Description = "Valid description",
            Status = TaskStatus.ToDo,
            Priority = TaskPriority.Medium,
            ProjectId = Guid.NewGuid()
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Should_Pass_When_Optional_Fields_Are_Null()
    {
        // Arrange
        var dto = new CreateTaskDto
        {
            Title = "Valid Task Title",
            Description = null,
            Status = TaskStatus.ToDo,
            Priority = TaskPriority.Medium,
            StartDate = null,
            DueDate = null,
            ProjectId = Guid.NewGuid(),
            ParentTaskId = null
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    #endregion

    #region Title Validation

    [Fact]
    public void Should_Fail_When_Title_Is_Empty()
    {
        // Arrange
        var dto = CreateValidDto();
        dto.Title = "";

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => 
            e.PropertyName == "Title" &&
            e.ErrorMessage == "Task title is required");
    }

    [Theory]
    [InlineData("ab")]
    [InlineData("a")]
    public void Should_Fail_When_Title_Is_Too_Short(string title)
    {
        // Arrange
        var dto = CreateValidDto();
        dto.Title = title;

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => 
            e.PropertyName == "Title" &&
            e.ErrorMessage == "Task title must be at least 3 characters");
    }

    [Fact]
    public void Should_Fail_When_Title_Exceeds_MaxLength()
    {
        // Arrange
        var dto = CreateValidDto();
        dto.Title = new string('a', 301);

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => 
            e.PropertyName == "Title" &&
            e.ErrorMessage == "Task title cannot exceed 300 characters");
    }

    [Theory]
    [InlineData("Implement user authentication")]
    [InlineData("Fix bug in payment system")]
    [InlineData("Update README.md")]
    public void Should_Pass_When_Title_Is_Valid(string title)
    {
        // Arrange
        var dto = CreateValidDto();
        dto.Title = title;

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    #endregion

    #region Description Validation

    [Fact]
    public void Should_Pass_When_Description_Is_Null()
    {
        // Arrange
        var dto = CreateValidDto();
        dto.Description = null;

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Should_Fail_When_Description_Exceeds_MaxLength()
    {
        // Arrange
        var dto = CreateValidDto();
        dto.Description = new string('a', 2001);

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => 
            e.PropertyName == "Description" &&
            e.ErrorMessage == "Description cannot exceed 2000 characters");
    }

    #endregion

    #region Status Validation

    [Fact]
    public void Should_Pass_When_Status_Is_ToDo()
    {
        // Arrange
        var dto = CreateValidDto();
        dto.Status = TaskStatus.ToDo;

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Should_Pass_When_Status_Is_InProgress()
    {
        // Arrange
        var dto = CreateValidDto();
        dto.Status = TaskStatus.InProgress;

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Should_Pass_When_Status_Is_Done()
    {
        // Arrange
        var dto = CreateValidDto();
        dto.Status = TaskStatus.Done;

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Should_Fail_When_Status_Is_Invalid()
    {
        // Arrange
        var dto = CreateValidDto();
        dto.Status = (TaskStatus)999;

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => 
            e.PropertyName == "Status" &&
            e.ErrorMessage == "Invalid task status specified");
    }

    #endregion

    #region Priority Validation

    [Theory]
    [InlineData(TaskPriority.Low)]
    [InlineData(TaskPriority.Medium)]
    [InlineData(TaskPriority.High)]
    [InlineData(TaskPriority.Critical)]
    public void Should_Pass_When_Priority_Is_Valid(TaskPriority priority)
    {
        // Arrange
        var dto = CreateValidDto();
        dto.Priority = priority;

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Should_Fail_When_Priority_Is_Invalid()
    {
        // Arrange
        var dto = CreateValidDto();
        dto.Priority = (TaskPriority)999;

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => 
            e.PropertyName == "Priority" &&
            e.ErrorMessage == "Invalid task priority specified");
    }

    #endregion

    #region Date Validation

    [Fact]
    public void Should_Pass_When_StartDate_Equals_DueDate()
    {
        // Arrange
        var date = DateTime.Now.AddDays(1);
        var dto = CreateValidDto();
        dto.StartDate = date;
        dto.DueDate = date;

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Should_Pass_When_StartDate_Is_Before_DueDate()
    {
        // Arrange
        var dto = CreateValidDto();
        dto.StartDate = DateTime.Now;
        dto.DueDate = DateTime.Now.AddDays(7);

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Should_Fail_When_StartDate_Is_After_DueDate()
    {
        // Arrange
        var dto = CreateValidDto();
        dto.StartDate = DateTime.Now.AddDays(7);
        dto.DueDate = DateTime.Now;

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => 
            e.PropertyName == "StartDate" &&
            e.ErrorMessage == "Start date must be before or equal to due date");
    }

    [Fact]
    public void Should_Pass_When_Only_StartDate_Is_Provided()
    {
        // Arrange
        var dto = CreateValidDto();
        dto.StartDate = DateTime.Now;
        dto.DueDate = null;

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Should_Pass_When_Only_DueDate_Is_Provided()
    {
        // Arrange
        var dto = CreateValidDto();
        dto.StartDate = null;
        dto.DueDate = DateTime.Now.AddDays(7);

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    #endregion

    #region ProjectId Validation

    [Fact]
    public void Should_Fail_When_ProjectId_Is_Empty()
    {
        // Arrange
        var dto = CreateValidDto();
        dto.ProjectId = Guid.Empty;

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => 
            e.PropertyName == "ProjectId" &&
            e.ErrorMessage == "Project ID is required");
    }

    [Fact]
    public void Should_Pass_When_ProjectId_Is_Valid_Guid()
    {
        // Arrange
        var dto = CreateValidDto();
        dto.ProjectId = Guid.NewGuid();

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    #endregion

    #region ParentTaskId Validation

    [Fact]
    public void Should_Pass_When_ParentTaskId_Is_Null()
    {
        // Arrange
        var dto = CreateValidDto();
        dto.ParentTaskId = null;

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Should_Pass_When_ParentTaskId_Is_Valid_Guid()
    {
        // Arrange
        var dto = CreateValidDto();
        dto.ParentTaskId = Guid.NewGuid();

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Should_Fail_When_ParentTaskId_Equals_ProjectId()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var dto = CreateValidDto();
        dto.ProjectId = guid;
        dto.ParentTaskId = guid;

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => 
            e.PropertyName == "ParentTaskId" &&
            e.ErrorMessage == "Parent task cannot be the same as the project");
    }

    #endregion

    #region Helper Methods

    private CreateTaskDto CreateValidDto()
    {
        return new CreateTaskDto
        {
            Title = "Valid Task Title",
            Description = "Valid description",
            Status = TaskStatus.ToDo,
            Priority = TaskPriority.Medium,
            ProjectId = Guid.NewGuid()
        };
    }

    #endregion
}
