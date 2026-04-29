using FluentAssertions;
using TaskCraft.Application.DTOs.Project;
using TaskCraft.Application.Validators.Project;
using Xunit;

namespace TaskCraft.Tests.Validators.Project;

/// <summary>
/// Unit tests for CreateProjectDtoValidator
/// </summary>
public class CreateProjectDtoValidatorTests
{
    private readonly CreateProjectDtoValidator _validator;

    public CreateProjectDtoValidatorTests()
    {
        _validator = new CreateProjectDtoValidator();
    }

    #region Valid Cases

    [Fact]
    public void Should_Pass_When_All_Required_Fields_Are_Valid()
    {
        // Arrange
        var dto = new CreateProjectDto
        {
            Name = "TaskCraft Project",
            Description = "A comprehensive task management system",
            OwnerId = Guid.NewGuid()
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Should_Pass_When_Description_Is_Null()
    {
        // Arrange
        var dto = new CreateProjectDto
        {
            Name = "TaskCraft Project",
            Description = null,
            OwnerId = Guid.NewGuid()
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    #endregion

    #region Name Validation

    [Fact]
    public void Should_Fail_When_Name_Is_Empty()
    {
        // Arrange
        var dto = CreateValidDto();
        dto.Name = "";

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => 
            e.PropertyName == "Name" &&
            e.ErrorMessage == "Project name is required");
    }

    [Theory]
    [InlineData("ab")]
    [InlineData("a")]
    public void Should_Fail_When_Name_Is_Too_Short(string name)
    {
        // Arrange
        var dto = CreateValidDto();
        dto.Name = name;

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => 
            e.PropertyName == "Name" &&
            e.ErrorMessage == "Project name must be at least 3 characters");
    }

    [Fact]
    public void Should_Fail_When_Name_Exceeds_MaxLength()
    {
        // Arrange
        var dto = CreateValidDto();
        dto.Name = new string('a', 201);

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => 
            e.PropertyName == "Name" &&
            e.ErrorMessage == "Project name cannot exceed 200 characters");
    }

    [Theory]
    [InlineData("Project@Name")]
    [InlineData("Project#Name")]
    [InlineData("Project$Name")]
    [InlineData("Project&Name")]
    public void Should_Fail_When_Name_Contains_Invalid_Characters(string name)
    {
        // Arrange
        var dto = CreateValidDto();
        dto.Name = name;

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => 
            e.PropertyName == "Name" &&
            e.ErrorMessage == "Project name can only contain letters, numbers, spaces, hyphens, underscores, and periods");
    }

    [Theory]
    [InlineData("TaskCraft")]
    [InlineData("Task-Craft")]
    [InlineData("Task_Craft")]
    [InlineData("TaskCraft 2.0")]
    [InlineData("My Project 123")]
    public void Should_Pass_When_Name_Contains_Valid_Characters(string name)
    {
        // Arrange
        var dto = CreateValidDto();
        dto.Name = name;

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    #endregion

    #region Description Validation

    [Fact]
    public void Should_Fail_When_Description_Exceeds_MaxLength()
    {
        // Arrange
        var dto = CreateValidDto();
        dto.Description = new string('a', 1001);

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => 
            e.PropertyName == "Description" &&
            e.ErrorMessage == "Description cannot exceed 1000 characters");
    }

    [Fact]
    public void Should_Pass_When_Description_Is_At_MaxLength()
    {
        // Arrange
        var dto = CreateValidDto();
        dto.Description = new string('a', 1000);

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    #endregion

    #region OwnerId Validation

    [Fact]
    public void Should_Fail_When_OwnerId_Is_Empty()
    {
        // Arrange
        var dto = CreateValidDto();
        dto.OwnerId = Guid.Empty;

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => 
            e.PropertyName == "OwnerId" &&
            e.ErrorMessage == "Owner ID is required");
    }

    [Fact]
    public void Should_Pass_When_OwnerId_Is_Valid_Guid()
    {
        // Arrange
        var dto = CreateValidDto();
        dto.OwnerId = Guid.NewGuid();

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    #endregion

    #region Multiple Errors

    [Fact]
    public void Should_Return_Multiple_Errors_When_Multiple_Fields_Are_Invalid()
    {
        // Arrange
        var dto = new CreateProjectDto
        {
            Name = "ab",
            Description = new string('a', 1001),
            OwnerId = Guid.Empty
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
        result.Errors.Should().Contain(e => e.PropertyName == "Description");
        result.Errors.Should().Contain(e => e.PropertyName == "OwnerId");
    }

    #endregion

    #region Helper Methods

    private CreateProjectDto CreateValidDto()
    {
        return new CreateProjectDto
        {
            Name = "TaskCraft Project",
            Description = "A comprehensive task management system",
            OwnerId = Guid.NewGuid()
        };
    }

    #endregion
}
