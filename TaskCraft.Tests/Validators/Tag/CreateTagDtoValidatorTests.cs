using FluentAssertions;
using TaskCraft.Application.DTOs.Tag;
using TaskCraft.Application.Validators.Tag;
using Xunit;

namespace TaskCraft.Tests.Validators.Tag;

/// <summary>
/// Unit tests for CreateTagDtoValidator
/// </summary>
public class CreateTagDtoValidatorTests
{
    private readonly CreateTagDtoValidator _validator;

    public CreateTagDtoValidatorTests()
    {
        _validator = new CreateTagDtoValidator();
    }

    #region Valid Cases

    [Fact]
    public void Should_Pass_When_All_Fields_Are_Valid()
    {
        // Arrange
        var dto = new CreateTagDto
        {
            Name = "Important",
            Color = "#FF5733",
            ProjectId = Guid.NewGuid()
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Should_Pass_When_Color_Is_Null()
    {
        // Arrange
        var dto = new CreateTagDto
        {
            Name = "Bug",
            Color = null,
            ProjectId = Guid.NewGuid()
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
            e.ErrorMessage == "Tag name is required");
    }

    [Fact]
    public void Should_Pass_When_Name_Is_Single_Character()
    {
        // Arrange
        var dto = CreateValidDto();
        dto.Name = "A";

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Should_Fail_When_Name_Exceeds_MaxLength()
    {
        // Arrange
        var dto = CreateValidDto();
        dto.Name = new string('a', 51);

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => 
            e.PropertyName == "Name" &&
            e.ErrorMessage == "Tag name cannot exceed 50 characters");
    }

    [Fact]
    public void Should_Pass_When_Name_Is_At_MaxLength()
    {
        // Arrange
        var dto = CreateValidDto();
        dto.Name = new string('a', 50);

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("Bug@")]
    [InlineData("Feature#")]
    [InlineData("Task$")]
    [InlineData("High Priority!")]
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
            e.ErrorMessage == "Tag name can only contain letters, numbers, spaces, hyphens, and underscores");
    }

    [Theory]
    [InlineData("Bug")]
    [InlineData("Feature")]
    [InlineData("High-Priority")]
    [InlineData("Work_In_Progress")]
    [InlineData("Task 123")]
    [InlineData("v2-Release")]
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

    #region Color Validation

    [Theory]
    [InlineData("#FF5733")]
    [InlineData("#00FF00")]
    [InlineData("#FFFFFF")]
    [InlineData("#000000")]
    [InlineData("#AbCdEf")]
    [InlineData("#123456")]
    public void Should_Pass_When_Color_Is_Valid_Hex(string color)
    {
        // Arrange
        var dto = CreateValidDto();
        dto.Color = color;

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("red")]
    [InlineData("FF5733")]
    [InlineData("#FF57")]
    [InlineData("#FF57333")]
    [InlineData("FF5733#")]
    [InlineData("#GG5733")]
    public void Should_Fail_When_Color_Is_Invalid_Hex(string color)
    {
        // Arrange
        var dto = CreateValidDto();
        dto.Color = color;

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => 
            e.PropertyName == "Color" &&
            e.ErrorMessage == "Color must be in hex format (e.g., #FF5733)");
    }

    [Fact]
    public void Should_Pass_When_Color_Is_Empty_String()
    {
        // Arrange
        var dto = CreateValidDto();
        dto.Color = "";

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Should_Pass_When_Color_Is_Whitespace()
    {
        // Arrange
        var dto = CreateValidDto();
        dto.Color = "   ";

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

    #region Multiple Errors

    [Fact]
    public void Should_Return_Multiple_Errors_When_Multiple_Fields_Are_Invalid()
    {
        // Arrange
        var dto = new CreateTagDto
        {
            Name = "",
            Color = "invalid",
            ProjectId = Guid.Empty
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
        result.Errors.Should().Contain(e => e.PropertyName == "Color");
        result.Errors.Should().Contain(e => e.PropertyName == "ProjectId");
    }

    #endregion

    #region Common Tag Names and Colors

    [Theory]
    [InlineData("Bug", "#FF0000")]
    [InlineData("Feature", "#00FF00")]
    [InlineData("Enhancement", "#0000FF")]
    [InlineData("Documentation", "#FFFF00")]
    [InlineData("Critical", "#FF00FF")]
    [InlineData("Low Priority", "#808080")]
    public void Should_Pass_For_Common_Tag_Combinations(string name, string color)
    {
        // Arrange
        var dto = new CreateTagDto
        {
            Name = name,
            Color = color,
            ProjectId = Guid.NewGuid()
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    #endregion

    #region Helper Methods

    private CreateTagDto CreateValidDto()
    {
        return new CreateTagDto
        {
            Name = "Important",
            Color = "#FF5733",
            ProjectId = Guid.NewGuid()
        };
    }

    #endregion
}
