# ✅ Unit Tests - TaskCraft Validation Layer

## 📊 Test Summary

**Total Tests: 199** ✅  
**Passing: 199** 🟢  
**Failing: 0** 🔴  
**Coverage: 100%** of validators

---

## 🎯 Test Organization

```
TaskCraft.Tests/
├── Validators/
│   ├── Auth/
│   │   ├── LoginRequestDtoValidatorTests.cs (25 tests)
│   │   └── RegisterRequestDtoValidatorTests.cs (48 tests)
│   ├── User/
│   │   └── CreateUserDtoValidatorTests.cs (31 tests)
│   ├── Project/
│   │   └── CreateProjectDtoValidatorTests.cs (21 tests)
│   ├── Task/
│   │   └── CreateTaskDtoValidatorTests.cs (39 tests)
│   ├── Comment/
│   │   └── CreateCommentDtoValidatorTests.cs (20 tests)
│   └── Tag/
│       └── CreateTagDtoValidatorTests.cs (15 tests)
```

---

## 📚 Test Coverage by Validator

### 🔐 Auth Validators (73 tests)

#### **LoginRequestDtoValidator** - 25 tests
- ✅ Valid login scenarios
- ✅ Username validation (empty, too short, too long)
- ✅ Password validation (empty, too short)
- ✅ IP address validation (valid/invalid formats)

#### **RegisterRequestDtoValidator** - 48 tests
- ✅ Valid registration scenarios
- ✅ Username validation (length, special characters, format)
- ✅ Email validation (format, length)
- ✅ Password complexity (length, uppercase, lowercase, numbers, special chars)
- ✅ First/Last name validation (format, length, characters)
- ✅ IP address validation
- ✅ Multiple simultaneous errors

---

### 👤 User Validators (31 tests)

#### **CreateUserDtoValidator** - 31 tests
- ✅ Regular user creation with password
- ✅ Anonymous user creation without password
- ✅ Username validation
- ✅ Email validation
- ✅ Password requirements for non-anonymous users
- ✅ Password optional for anonymous users
- ✅ Role validation (Admin, User, invalid values)

---

### 📁 Project Validators (21 tests)

#### **CreateProjectDtoValidator** - 21 tests
- ✅ Valid project creation
- ✅ Name validation (length, special characters, format)
- ✅ Description validation (max length)
- ✅ Owner ID validation
- ✅ Multiple errors handling

---

### ✅ Task Validators (39 tests)

#### **CreateTaskDtoValidator** - 39 tests
- ✅ Valid task creation
- ✅ Title validation (length, required)
- ✅ Description validation (max length, optional)
- ✅ Status validation (all enum values, invalid values)
- ✅ Priority validation (Low, Medium, High, Critical)
- ✅ Date validation (StartDate ≤ DueDate logic)
- ✅ Project ID validation
- ✅ Parent Task ID validation

---

### 💬 Comment Validators (20 tests)

#### **CreateCommentDtoValidator** - 20 tests
- ✅ Comments with TaskId
- ✅ Comments with ProjectId
- ✅ Comments with both IDs
- ✅ Content validation (length, required)
- ✅ User ID validation
- ✅ At least one ID (TaskId or ProjectId) required
- ✅ Multiple errors handling

---

### 🏷️ Tag Validators (15 tests)

#### **CreateTagDtoValidator** - 15 tests
- ✅ Valid tag creation
- ✅ Name validation (length, characters)
- ✅ Color validation (hex format #RRGGBB)
- ✅ Project ID validation
- ✅ Optional color field
- ✅ Common tag combinations

---

## 🧪 Test Patterns Used

### 1. **Fact Tests** (single scenario)
```csharp
[Fact]
public void Should_Pass_When_All_Fields_Are_Valid()
{
    // Arrange
    var dto = CreateValidDto();

    // Act
    var result = _validator.Validate(dto);

    // Assert
    result.IsValid.Should().BeTrue();
    result.Errors.Should().BeEmpty();
}
```

### 2. **Theory Tests** (multiple scenarios)
```csharp
[Theory]
[InlineData("ValidPass123!")]
[InlineData("MyP@ssw0rd")]
[InlineData("Str0ng#Pass")]
public void Should_Pass_When_Password_Meets_Requirements(string password)
{
    // Arrange
    var dto = CreateValidDto();
    dto.Password = password;

    // Act
    var result = _validator.Validate(dto);

    // Assert
    result.IsValid.Should().BeTrue();
}
```

### 3. **FluentAssertions** (readable assertions)
```csharp
result.IsValid.Should().BeFalse();
result.Errors.Should().Contain(e => 
    e.PropertyName == "Email" &&
    e.ErrorMessage == "Invalid email format");
```

---

## 🚀 Running Tests

### Run All Tests
```bash
cd C:\Users\Hola\source\repos
dotnet test
```

### Run Specific Test Class
```bash
dotnet test --filter "FullyQualifiedName~LoginRequestDtoValidatorTests"
```

### Run Tests with Detailed Output
```bash
dotnet test --logger "console;verbosity=detailed"
```

### Run Tests by Category
```bash
# Auth tests only
dotnet test --filter "FullyQualifiedName~Auth"

# User tests only
dotnet test --filter "FullyQualifiedName~User"
```

---

## 📊 Test Execution Results

```
Resumen de pruebas: 
  Total: 199
  Con errores: 0 ✅
  Correcto: 199 🟢
  Omitido: 0
  Duración: ~2 segundos
```

---

## 🛠️ Technologies Used

- **xUnit** - Test framework
- **FluentAssertions** - Assertion library for readable tests
- **FluentValidation** - Validation library being tested
- **.NET 10** - Target framework

---

## 📖 Test Examples

### Example 1: Testing Password Complexity

```csharp
[Fact]
public void Should_Fail_When_Password_Has_No_Uppercase()
{
    // Arrange
    var dto = new RegisterRequestDto
    {
        Username = "testuser",
        Email = "test@example.com",
        Password = "password123!", // ❌ No uppercase
        FirstName = "John",
        LastName = "Doe"
    };

    // Act
    var result = _validator.Validate(dto);

    // Assert
    result.IsValid.Should().BeFalse();
    result.Errors.Should().Contain(e => 
        e.PropertyName == "Password" &&
        e.ErrorMessage == "Password must contain at least one uppercase letter");
}
```

### Example 2: Testing Date Logic

```csharp
[Fact]
public void Should_Fail_When_StartDate_Is_After_DueDate()
{
    // Arrange
    var dto = new CreateTaskDto
    {
        Title = "My Task",
        StartDate = DateTime.Now.AddDays(7),
        DueDate = DateTime.Now, // ❌ Before start date
        ProjectId = Guid.NewGuid()
    };

    // Act
    var result = _validator.Validate(dto);

    // Assert
    result.IsValid.Should().BeFalse();
    result.Errors.Should().Contain(e => 
        e.PropertyName == "StartDate" &&
        e.ErrorMessage == "Start date must be before or equal to due date");
}
```

### Example 3: Testing Multiple Errors

```csharp
[Fact]
public void Should_Return_Multiple_Errors_When_Multiple_Fields_Are_Invalid()
{
    // Arrange
    var dto = new RegisterRequestDto
    {
        Username = "ab",        // ❌ Too short
        Email = "invalid",      // ❌ Invalid format
        Password = "weak",      // ❌ Too weak
        FirstName = "",         // ❌ Required
        LastName = ""           // ❌ Required
    };

    // Act
    var result = _validator.Validate(dto);

    // Assert
    result.IsValid.Should().BeFalse();
    result.Errors.Should().Contain(e => e.PropertyName == "Username");
    result.Errors.Should().Contain(e => e.PropertyName == "Email");
    result.Errors.Should().Contain(e => e.PropertyName == "Password");
    result.Errors.Should().Contain(e => e.PropertyName == "FirstName");
    result.Errors.Should().Contain(e => e.PropertyName == "LastName");
}
```

---

## 🎯 Best Practices Followed

1. ✅ **AAA Pattern**: Arrange-Act-Assert in all tests
2. ✅ **Descriptive Names**: Test names clearly describe what's being tested
3. ✅ **One Concept Per Test**: Each test verifies one specific behavior
4. ✅ **Helper Methods**: `CreateValidDto()` reduces duplication
5. ✅ **Theory Data**: Parameterized tests for similar scenarios
6. ✅ **Clear Assertions**: FluentAssertions makes tests readable
7. ✅ **Complete Coverage**: All validation rules tested
8. ✅ **Edge Cases**: Boundary values, null values, empty strings

---

## 🔍 Continuous Integration

These tests should be run:
- ✅ Before every commit (pre-commit hook)
- ✅ On every pull request (CI pipeline)
- ✅ Before deployment (release pipeline)

### GitHub Actions Example
```yaml
name: Tests
on: [push, pull_request]
jobs:
  test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v2
      - name: Setup .NET
        uses: actions/setup-dotnet@v1
        with:
          dotnet-version: '10.0.x'
      - name: Run Tests
        run: dotnet test
```

---

## 📈 Future Improvements

1. **Code Coverage Reports**: Integrate with Coverlet
2. **Performance Tests**: Add benchmark tests for validators
3. **Integration Tests**: Test validators with actual API endpoints
4. **Mutation Testing**: Verify test quality with Stryker.NET

---

## 🎉 Success Metrics

- ✅ **100% validator coverage**
- ✅ **199 tests passing**
- ✅ **Fast execution** (~2 seconds)
- ✅ **Maintainable** - easy to add new tests
- ✅ **Reliable** - consistent results

---

## 📚 Related Documentation

- [Validation Layer Guide](../TaskCraft.API/Docs/VALIDATION_GUIDE.md)
- [Validation Implementation Summary](../TaskCraft.API/Docs/VALIDATION_IMPLEMENTATION_SUMMARY.md)
- [xUnit Documentation](https://xunit.net/)
- [FluentAssertions Documentation](https://fluentassertions.com/)

---

**Tests Written With ❤️ for TaskCraft**

*Last Updated: 2024*  
*Test Framework: xUnit + FluentAssertions*  
*Status: All Tests Passing ✅*
