using FluentAssertions;
using TaskCraft.Application.DTOs.Auth;
using TaskCraft.Application.Validators.Auth;
using Xunit;

namespace TaskCraft.Tests.Validators.Auth;

/// <summary>
/// Unit tests for LoginRequestDtoValidator
/// </summary>
public class LoginRequestDtoValidatorTests
{
    private readonly LoginRequestDtoValidator _validator;

    public LoginRequestDtoValidatorTests()
    {
        _validator = new LoginRequestDtoValidator();
    }

    #region Valid Cases

    [Fact]
    public void Should_Pass_When_All_Fields_Are_Valid()
    {
        // Arrange
        var dto = new LoginRequestDto
        {
            Username = "testuser",
            Password = "Password123",
            IpAddress = null
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Should_Pass_When_IpAddress_Is_Provided_And_Valid()
    {
        // Arrange
        var dto = new LoginRequestDto
        {
            Username = "testuser",
            Password = "Password123",
            IpAddress = "192.168.1.100"
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
        var dto = new LoginRequestDto
        {
            Username = "",
            Password = "Password123"
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => 
            e.PropertyName == "Username" && 
            e.ErrorMessage == "Username is required");
    }

    [Fact]
    public void Should_Fail_When_Username_Is_Too_Short()
    {
        // Arrange
        var dto = new LoginRequestDto
        {
            Username = "ab",
            Password = "Password123"
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => 
            e.PropertyName == "Username" && 
            e.ErrorMessage == "Username must be at least 3 characters");
    }

    [Fact]
    public void Should_Fail_When_Username_Is_Too_Long()
    {
        // Arrange
        var dto = new LoginRequestDto
        {
            Username = new string('a', 101),
            Password = "Password123"
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => 
            e.PropertyName == "Username" && 
            e.ErrorMessage == "Username cannot exceed 100 characters");
    }

    #endregion

    #region Password Validation

    [Fact]
    public void Should_Fail_When_Password_Is_Empty()
    {
        // Arrange
        var dto = new LoginRequestDto
        {
            Username = "testuser",
            Password = ""
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => 
            e.PropertyName == "Password" && 
            e.ErrorMessage == "Password is required");
    }

    [Fact]
    public void Should_Fail_When_Password_Is_Too_Short()
    {
        // Arrange
        var dto = new LoginRequestDto
        {
            Username = "testuser",
            Password = "Pass1"
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => 
            e.PropertyName == "Password" && 
            e.ErrorMessage == "Password must be at least 6 characters");
    }

    #endregion

    #region IpAddress Validation

    [Fact]
    public void Should_Fail_When_IpAddress_Is_Invalid()
    {
        // Arrange
        var dto = new LoginRequestDto
        {
            Username = "testuser",
            Password = "Password123",
            IpAddress = "invalid-ip"
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => 
            e.PropertyName == "IpAddress" && 
            e.ErrorMessage == "Invalid IP address format");
    }

    [Theory]
    [InlineData("invalid-ip")]
    [InlineData("abc.def.ghi.jkl")]
    [InlineData("192.168.1")]
    [InlineData("192.168.1.1.1")]
    public void Should_Fail_When_IpAddress_Is_Invalid_Format(string ipAddress)
    {
        // Arrange
        var dto = new LoginRequestDto
        {
            Username = "testuser",
            Password = "Password123",
            IpAddress = ipAddress
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "IpAddress");
    }

    [Theory]
    [InlineData("0.0.0.0")]
    [InlineData("127.0.0.1")]
    [InlineData("192.168.1.1")]
    [InlineData("255.255.255.255")]
    public void Should_Pass_When_IpAddress_Is_Valid(string ipAddress)
    {
        // Arrange
        var dto = new LoginRequestDto
        {
            Username = "testuser",
            Password = "Password123",
            IpAddress = ipAddress
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    #endregion
}
