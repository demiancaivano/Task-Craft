using FluentAssertions;
using TaskCraft.Application.DTOs.Auth;
using TaskCraft.Application.Validators.Auth;
using Xunit;

namespace TaskCraft.Tests.Validators.Auth;

/// <summary>
/// Unit tests for RegisterRequestDtoValidator
/// </summary>
public class RegisterRequestDtoValidatorTests
{
    private readonly RegisterRequestDtoValidator _validator;

    public RegisterRequestDtoValidatorTests()
    {
        _validator = new RegisterRequestDtoValidator();
    }

    #region Valid Cases

    [Fact]
    public void Should_Pass_When_All_Fields_Are_Valid()
    {
        // Arrange
        var dto = new RegisterRequestDto
        {
            Username = "johndoe",
            Email = "john.doe@example.com",
            Password = "SecurePass123!",
            FirstName = "John",
            LastName = "Doe"
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
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
        result.Errors.Should().Contain(e => 
            e.PropertyName == "Username" && 
            e.ErrorMessage == "Username is required");
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
        result.Errors.Should().Contain(e => 
            e.PropertyName == "Username" && 
            e.ErrorMessage == "Username must be at least 3 characters");
    }

    [Fact]
    public void Should_Fail_When_Username_Exceeds_MaxLength()
    {
        // Arrange
        var dto = CreateValidDto();
        dto.Username = new string('a', 101);

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => 
            e.PropertyName == "Username" && 
            e.ErrorMessage == "Username cannot exceed 100 characters");
    }

    [Theory]
    [InlineData("user name")]
    [InlineData("user@name")]
    [InlineData("user#name")]
    [InlineData("user.name")]
    public void Should_Fail_When_Username_Contains_Invalid_Characters(string username)
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

    [Theory]
    [InlineData("user123")]
    [InlineData("user_name")]
    [InlineData("user-name")]
    [InlineData("User123")]
    [InlineData("USER_NAME")]
    public void Should_Pass_When_Username_Contains_Valid_Characters(string username)
    {
        // Arrange
        var dto = CreateValidDto();
        dto.Username = username;

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeTrue();
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
        result.Errors.Should().Contain(e => 
            e.PropertyName == "Email" && 
            e.ErrorMessage == "Email is required");
    }

    [Theory]
    [InlineData("invalid")]
    [InlineData("invalid@")]
    [InlineData("@invalid.com")]
    [InlineData("invalid.com")]
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

    [Fact]
    public void Should_Fail_When_Email_Exceeds_MaxLength()
    {
        // Arrange
        var dto = CreateValidDto();
        dto.Email = new string('a', 250) + "@test.com";

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => 
            e.PropertyName == "Email" && 
            e.ErrorMessage == "Email cannot exceed 255 characters");
    }

    [Theory]
    [InlineData("test@example.com")]
    [InlineData("user.name@example.com")]
    [InlineData("user+tag@example.co.uk")]
    public void Should_Pass_When_Email_Is_Valid(string email)
    {
        // Arrange
        var dto = CreateValidDto();
        dto.Email = email;

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    #endregion

    #region Password Validation

    [Fact]
    public void Should_Fail_When_Password_Is_Empty()
    {
        // Arrange
        var dto = CreateValidDto();
        dto.Password = "";

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

    [Fact]
    public void Should_Fail_When_Password_Has_No_SpecialCharacter()
    {
        // Arrange
        var dto = CreateValidDto();
        dto.Password = "Password123";

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => 
            e.PropertyName == "Password" && 
            e.ErrorMessage == "Password must contain at least one special character");
    }

    [Theory]
    [InlineData("ValidPass123!")]
    [InlineData("MyP@ssw0rd")]
    [InlineData("Str0ng#Pass")]
    [InlineData("Test$123Pass")]
    public void Should_Pass_When_Password_Meets_All_Requirements(string password)
    {
        // Arrange
        var dto = CreateValidDto();
        dto.Password = password;

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    #endregion

    #region FirstName Validation

    [Fact]
    public void Should_Fail_When_FirstName_Is_Empty()
    {
        // Arrange
        var dto = CreateValidDto();
        dto.FirstName = "";

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => 
            e.PropertyName == "FirstName" && 
            e.ErrorMessage == "First name is required");
    }

    [Fact]
    public void Should_Fail_When_FirstName_Exceeds_MaxLength()
    {
        // Arrange
        var dto = CreateValidDto();
        dto.FirstName = new string('a', 101);

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => 
            e.PropertyName == "FirstName" && 
            e.ErrorMessage == "First name cannot exceed 100 characters");
    }

    [Theory]
    [InlineData("John123")]
    [InlineData("John@")]
    [InlineData("John_Doe")]
    public void Should_Fail_When_FirstName_Contains_Invalid_Characters(string firstName)
    {
        // Arrange
        var dto = CreateValidDto();
        dto.FirstName = firstName;

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => 
            e.PropertyName == "FirstName" && 
            e.ErrorMessage == "First name can only contain letters, spaces, hyphens, and apostrophes");
    }

    [Theory]
    [InlineData("John")]
    [InlineData("Mary-Jane")]
    [InlineData("O'Brien")]
    [InlineData("María")]
    [InlineData("José")]
    [InlineData("Jean-Pierre")]
    public void Should_Pass_When_FirstName_Is_Valid(string firstName)
    {
        // Arrange
        var dto = CreateValidDto();
        dto.FirstName = firstName;

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    #endregion

    #region LastName Validation

    [Fact]
    public void Should_Fail_When_LastName_Is_Empty()
    {
        // Arrange
        var dto = CreateValidDto();
        dto.LastName = "";

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => 
            e.PropertyName == "LastName" && 
            e.ErrorMessage == "Last name is required");
    }

    [Theory]
    [InlineData("Smith")]
    [InlineData("O'Connor")]
    [InlineData("García")]
    [InlineData("Van Der Berg")]
    public void Should_Pass_When_LastName_Is_Valid(string lastName)
    {
        // Arrange
        var dto = CreateValidDto();
        dto.LastName = lastName;

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
        var dto = new RegisterRequestDto
        {
            Username = "ab",
            Email = "invalid",
            Password = "weak",
            FirstName = "",
            LastName = ""
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
        result.Errors.Should().Contain(e => e.PropertyName == "Username");
        result.Errors.Should().Contain(e => e.PropertyName == "Email");
        result.Errors.Should().Contain(e => e.PropertyName == "Password");
        result.Errors.Should().Contain(e => e.PropertyName == "FirstName");
        result.Errors.Should().Contain(e => e.PropertyName == "LastName");
    }

    #endregion

    #region Helper Methods

    private RegisterRequestDto CreateValidDto()
    {
        return new RegisterRequestDto
        {
            Username = "johndoe",
            Email = "john.doe@example.com",
            Password = "SecurePass123!",
            FirstName = "John",
            LastName = "Doe"
        };
    }

    #endregion
}
