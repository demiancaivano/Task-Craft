using FluentAssertions;
using TaskCraft.Application.DTOs.Comment;
using TaskCraft.Application.Validators.Comment;
using Xunit;

namespace TaskCraft.Tests.Validators.Comment;

/// <summary>
/// Unit tests for CreateCommentDtoValidator
/// </summary>
public class CreateCommentDtoValidatorTests
{
    private readonly CreateCommentDtoValidator _validator;

    public CreateCommentDtoValidatorTests()
    {
        _validator = new CreateCommentDtoValidator();
    }

    #region Valid Cases

    [Fact]
    public void Should_Pass_When_Comment_Has_TaskId()
    {
        // Arrange
        var dto = new CreateCommentDto
        {
            Content = "This is a valid comment",
            UserId = Guid.NewGuid(),
            TaskId = Guid.NewGuid(),
            ProjectId = null
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Should_Pass_When_Comment_Has_ProjectId()
    {
        // Arrange
        var dto = new CreateCommentDto
        {
            Content = "This is a valid comment",
            UserId = Guid.NewGuid(),
            TaskId = null,
            ProjectId = Guid.NewGuid()
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Should_Pass_When_Comment_Has_Both_TaskId_And_ProjectId()
    {
        // Arrange
        var dto = new CreateCommentDto
        {
            Content = "This is a valid comment",
            UserId = Guid.NewGuid(),
            TaskId = Guid.NewGuid(),
            ProjectId = Guid.NewGuid()
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    #endregion

    #region Content Validation

    [Fact]
    public void Should_Fail_When_Content_Is_Empty()
    {
        // Arrange
        var dto = CreateValidDto();
        dto.Content = "";

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => 
            e.PropertyName == "Content" &&
            e.ErrorMessage == "Comment content is required");
    }

    [Fact]
    public void Should_Pass_When_Content_Is_Single_Character()
    {
        // Arrange
        var dto = CreateValidDto();
        dto.Content = "!";

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Should_Fail_When_Content_Exceeds_MaxLength()
    {
        // Arrange
        var dto = CreateValidDto();
        dto.Content = new string('a', 2001);

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => 
            e.PropertyName == "Content" &&
            e.ErrorMessage == "Comment cannot exceed 2000 characters");
    }

    [Fact]
    public void Should_Pass_When_Content_Is_At_MaxLength()
    {
        // Arrange
        var dto = CreateValidDto();
        dto.Content = new string('a', 2000);

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("Great work! 👍")]
    [InlineData("This task needs more clarification.")]
    [InlineData("LGTM")]
    [InlineData("Fixed in commit abc123")]
    public void Should_Pass_When_Content_Is_Valid(string content)
    {
        // Arrange
        var dto = CreateValidDto();
        dto.Content = content;

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    #endregion

    #region UserId Validation

    [Fact]
    public void Should_Fail_When_UserId_Is_Empty()
    {
        // Arrange
        var dto = CreateValidDto();
        dto.UserId = Guid.Empty;

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => 
            e.PropertyName == "UserId" &&
            e.ErrorMessage == "User ID is required");
    }

    [Fact]
    public void Should_Pass_When_UserId_Is_Valid_Guid()
    {
        // Arrange
        var dto = CreateValidDto();
        dto.UserId = Guid.NewGuid();

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    #endregion

    #region TaskId and ProjectId Validation

    [Fact]
    public void Should_Fail_When_Both_TaskId_And_ProjectId_Are_Null()
    {
        // Arrange
        var dto = new CreateCommentDto
        {
            Content = "This is a comment",
            UserId = Guid.NewGuid(),
            TaskId = null,
            ProjectId = null
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => 
            e.ErrorMessage == "Either TaskId or ProjectId must be provided");
    }

    [Fact]
    public void Should_Pass_When_Only_TaskId_Is_Provided()
    {
        // Arrange
        var dto = new CreateCommentDto
        {
            Content = "Task comment",
            UserId = Guid.NewGuid(),
            TaskId = Guid.NewGuid(),
            ProjectId = null
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Should_Pass_When_Only_ProjectId_Is_Provided()
    {
        // Arrange
        var dto = new CreateCommentDto
        {
            Content = "Project comment",
            UserId = Guid.NewGuid(),
            TaskId = null,
            ProjectId = Guid.NewGuid()
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Should_Pass_When_TaskId_Is_Empty_Guid_But_ProjectId_Exists()
    {
        // Arrange
        var dto = new CreateCommentDto
        {
            Content = "Comment",
            UserId = Guid.NewGuid(),
            TaskId = Guid.Empty,
            ProjectId = Guid.NewGuid()
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        // Empty GUID is technically valid when the other ID is provided
        // The validator only checks if at least one is provided
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Should_Pass_When_ProjectId_Is_Empty_Guid_But_TaskId_Exists()
    {
        // Arrange
        var dto = new CreateCommentDto
        {
            Content = "Comment",
            UserId = Guid.NewGuid(),
            TaskId = Guid.NewGuid(),
            ProjectId = Guid.Empty
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        // Empty GUID is technically valid when the other ID is provided
        result.IsValid.Should().BeTrue();
    }

    #endregion

    #region Multiple Errors

    [Fact]
    public void Should_Return_Multiple_Errors_When_Multiple_Fields_Are_Invalid()
    {
        // Arrange
        var dto = new CreateCommentDto
        {
            Content = "",
            UserId = Guid.Empty,
            TaskId = null,
            ProjectId = null
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Content");
        result.Errors.Should().Contain(e => e.PropertyName == "UserId");
        result.Errors.Should().Contain(e => e.ErrorMessage == "Either TaskId or ProjectId must be provided");
    }

    #endregion

    #region Helper Methods

    private CreateCommentDto CreateValidDto()
    {
        return new CreateCommentDto
        {
            Content = "This is a valid comment",
            UserId = Guid.NewGuid(),
            TaskId = Guid.NewGuid(),
            ProjectId = null
        };
    }

    #endregion
}
