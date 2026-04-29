using FluentAssertions;
using TaskCraft.Application.DTOs.User;
using TaskCraft.Application.Validators.User;
using TaskCraft.Core.Enums;
using Xunit;

namespace TaskCraft.Tests.Validators.User;

/// <summary>
/// Unit tests for CreateUserDtoValidator
/// </summary>
public class CreateUserDtoValidatorTests
{
    private readonly CreateUserDtoValidator _validator;

    public CreateUserDtoValidatorTests()
    {
        _validator = new CreateUserDtoValidator();
    }

    #region Valid Cases

    [Fact]
    public void Should_Pass_When_Creating_Regular_User_With_Password()
    {
        // Arrange
        var dto = new CreateUserDto
        {
            Username = "testuser",
            Email = "test@example.com",
            Password = "SecurePass123!",
            Role = UserRole.User,
            IsAnonymous = false
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Should_Pass_When_Creating_Anonymous_User_Without_Password()
    {
        // Arrange
        var dto = new CreateUserDto
        {
            Username = "guest_user",
            Email = "guest@example.com",
            Password = null,
            Role = UserRole.User,
            IsAnonymous = true
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    #endregion

    #region Username Validation

    [Fact]
    public void Should_Fail_When_Username_Is_Empty()
    {
        // Arrange
        var dto = CreateValidDto();
        dto.Username = "";

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Username");
    }

    [Theory]
    [InlineData("ab")]
    [InlineData("a")]
    public void Should_Fail_When_Username_Is_Too_Short(string username)
    {
        // Arrange
        var dto = CreateValidDto();
        dto.Username = username;

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Username");
    }

    [Theory]
    [InlineData("user name")]
    [InlineData("user@name")]
    [InlineData("user.name")]
    public void Should_Fail_When_Username_Has_Invalid_Characters(string username)
    {
        // Arrange
        var dto = CreateValidDto();
        dto.Username = username;

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => 
            e.PropertyName == "Username" &&
            e.ErrorMessage == "Username can only contain letters, numbers, underscores, and hyphens");
    }

    #endregion

    #region Email Validation

    [Fact]
    public void Should_Fail_When_Email_Is_Empty()
    {
        // Arrange
        var dto = CreateValidDto();
        dto.Email = "";

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Email");
    }

    [Theory]
    [InlineData("invalid")]
    [InlineData("invalid@")]
    [InlineData("@invalid.com")]
    public void Should_Fail_When_Email_Is_Invalid(string email)
    {
        // Arrange
        var dto = CreateValidDto();
        dto.Email = email;

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => 
            e.PropertyName == "Email" &&
            e.ErrorMessage == "Invalid email format");
    }

    #endregion

    #region Password Validation - Non-Anonymous Users

    [Fact]
    public void Should_Fail_When_NonAnonymous_User_Has_No_Password()
    {
        // Arrange
        var dto = new CreateUserDto
        {
            Username = "testuser",
            Email = "test@example.com",
            Password = null,
            Role = UserRole.User,
            IsAnonymous = false
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => 
            e.PropertyName == "Password" &&
            e.ErrorMessage == "Password is required for non-anonymous users");
    }

    [Fact]
    public void Should_Fail_When_NonAnonymous_User_Has_Empty_Password()
    {
        // Arrange
        var dto = new CreateUserDto
        {
            Username = "testuser",
            Email = "test@example.com",
            Password = "",
            Role = UserRole.User,
            IsAnonymous = false
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Password");
    }

    [Theory]
    [InlineData("Short1!")]
    [InlineData("Pass1!")]
    public void Should_Fail_When_Password_Is_Too_Short(string password)
    {
        // Arrange
        var dto = CreateValidDto();
        dto.Password = password;

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => 
            e.PropertyName == "Password" &&
            e.ErrorMessage == "Password must be at least 8 characters");
    }

    [Fact]
    public void Should_Fail_When_Password_Has_No_Uppercase()
    {
        // Arrange
        var dto = CreateValidDto();
        dto.Password = "password123!";

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => 
            e.PropertyName == "Password" &&
            e.ErrorMessage == "Password must contain at least one uppercase letter");
    }

    [Fact]
    public void Should_Fail_When_Password_Has_No_Lowercase()
    {
        // Arrange
        var dto = CreateValidDto();
        dto.Password = "PASSWORD123!";

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => 
            e.PropertyName == "Password" &&
            e.ErrorMessage == "Password must contain at least one lowercase letter");
    }

    [Fact]
    public void Should_Fail_When_Password_Has_No_Number()
    {
        // Arrange
        var dto = CreateValidDto();
        dto.Password = "PasswordTest!";

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => 
            e.PropertyName == "Password" &&
            e.ErrorMessage == "Password must contain at least one number");
    }

    #endregion

    #region Password Validation - Anonymous Users

    [Fact]
    public void Should_Pass_When_Anonymous_User_Has_No_Password()
    {
        // Arrange
        var dto = new CreateUserDto
        {
            Username = "guest123",
            Email = "guest@example.com",
            Password = null,
            Role = UserRole.User,
            IsAnonymous = true
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Should_Pass_When_Anonymous_User_Has_Empty_Password()
    {
        // Arrange
        var dto = new CreateUserDto
        {
            Username = "guest123",
            Email = "guest@example.com",
            Password = "",
            Role = UserRole.User,
            IsAnonymous = true
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    #endregion

    #region Role Validation

    [Fact]
    public void Should_Pass_When_Role_Is_Admin()
    {
        // Arrange
        var dto = CreateValidDto();
        dto.Role = UserRole.Admin;

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Should_Pass_When_Role_Is_User()
    {
        // Arrange
        var dto = CreateValidDto();
        dto.Role = UserRole.User;

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Should_Fail_When_Role_Is_Invalid()
    {
        // Arrange
        var dto = CreateValidDto();
        dto.Role = (UserRole)999; // Invalid enum value

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => 
            e.PropertyName == "Role" &&
            e.ErrorMessage == "Invalid role specified");
    }

    #endregion

    #region Helper Methods

    private CreateUserDto CreateValidDto()
    {
        return new CreateUserDto
        {
            Username = "testuser",
            Email = "test@example.com",
            Password = "SecurePass123!",
            Role = UserRole.User,
            IsAnonymous = false
        };
    }

    #endregion
}
