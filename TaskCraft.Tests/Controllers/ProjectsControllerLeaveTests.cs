using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using TaskCraft.API.Controllers;
using TaskCraft.Application.Interfaces;
using TaskCraft.Core.Exceptions;
using TaskCraft.Core.Interfaces;

namespace TaskCraft.Tests.Controllers;

public class ProjectsControllerLeaveTests
{
    private readonly Mock<IProjectService> _projectServiceMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<ILogger<ProjectsController>> _loggerMock = new();

    [Fact]
    public async Task LeaveProject_Should_ReturnNoContent_When_UserLeavesSuccessfully()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var controller = CreateController(userId);

        _projectServiceMock
            .Setup(s => s.RemoveMemberFromProjectAsync(projectId, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await controller.LeaveProject(projectId);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _projectServiceMock.Verify(
            s => s.RemoveMemberFromProjectAsync(projectId, userId, It.IsAny<CancellationToken>()),
            Times.Once);
        _unitOfWorkMock.Verify(
            u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task LeaveProject_Should_ReturnBadRequest_When_UserIsProjectOwner()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var controller = CreateController(userId);

        _projectServiceMock
            .Setup(s => s.RemoveMemberFromProjectAsync(projectId, userId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new BadRequestException("Cannot remove the project owner from the project"));

        // Act
        var result = await controller.LeaveProject(projectId);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        _unitOfWorkMock.Verify(
            u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task LeaveProject_Should_ReturnNotFound_When_MembershipDoesNotExist()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var controller = CreateController(userId);

        _projectServiceMock
            .Setup(s => s.RemoveMemberFromProjectAsync(projectId, userId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new KeyNotFoundException("Membership not found"));

        // Act
        var result = await controller.LeaveProject(projectId);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
        _unitOfWorkMock.Verify(
            u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }

    private ProjectsController CreateController(Guid userId)
    {
        var controller = new ProjectsController(
            _projectServiceMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object);

        var identity = new ClaimsIdentity(
            new[] { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) },
            "TestAuthType");

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(identity)
            }
        };

        return controller;
    }
}
