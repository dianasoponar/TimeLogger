using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using Timelogger.Api.Controllers;
using Timelogger.Api.DTOs;
using Timelogger.Api.Repositories;
using Timelogger.Entities;
using Xunit;

namespace Timelogger.Spi.Tests
{
    public class ProjectsControllerTests
    {
        private readonly Mock<IProjectsRepository> _mockRepository;
        private readonly ProjectsController _controller;

        public ProjectsControllerTests()
        {
            _mockRepository = new Mock<IProjectsRepository>();
            _controller = new ProjectsController(_mockRepository.Object);
        }

        [Fact]
        public void Get_ReturnsOkResult_WithProjects()
        {
            // Arrange
            var projects = new List<ProjectDto>
        {
            new ProjectDto { Id = "1", Name = "Project 1", Status = ProjectStatus.Completed },
            new ProjectDto { Id = "2", Name = "Project 2", Status = ProjectStatus.InProgress }
        };
            _mockRepository.Setup(repo => repo.GetAllProjects()).Returns(projects);

            // Act
            var result = _controller.Get();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnProjects = Assert.IsType<List<ProjectDto>>(okResult.Value);
            Assert.Equal(2, returnProjects.Count);
        }

        [Fact]
        public void AddTimeRegistration_ReturnsBadRequest_WhenBodyIsNull()
        {
            // Arrange
            int projectId = 3;
            TimeRegistrationDto body = null;

            // Act
            var result = _controller.AddTimeRegistration(projectId, body);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid time registration data.", badRequestResult.Value);
        }

        [Fact]
        public void AddTimeRegistration_ReturnsNotFound_WhenProjectDoesNotExist()
        {
            // Arrange
            var projectId = 4;
            var timeRegistrationDto = new TimeRegistrationDto { TimeSpent = TimeDuration.TwoHours };

            _mockRepository.Setup(repo => repo.GetProjectWithTimeRegistrations(projectId)).Returns((Project)null);

            // Act
            var result = _controller.AddTimeRegistration(projectId, timeRegistrationDto);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Project not found.", notFoundResult.Value);
        }

        [Fact]
        public void AddTimeRegistration_ReturnsOk_WhenProjectExists()
        {
            // Arrange
            var projectId = 5;
            var timeRegistrationDto = new TimeRegistrationDto { TimeSpent = TimeDuration.ThreeHours };
            var project = new Project
            {
                Id = projectId,
                Name = "Project 1",
                TimeRegistrations = new List<TimeRegistration>()
            };

            _mockRepository.Setup(repo => repo.GetProjectWithTimeRegistrations(projectId)).Returns(project);
            _mockRepository.Setup(repo => repo.AddTimeRegistration(projectId, It.IsAny<TimeRegistration>()));
            _mockRepository.Setup(repo => repo.Save());

            // Act
            var result = _controller.AddTimeRegistration(projectId, timeRegistrationDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(project, okResult.Value);
            _mockRepository.Verify(repo => repo.AddTimeRegistration(projectId, It.IsAny<TimeRegistration>()), Times.Once);
            _mockRepository.Verify(repo => repo.Save(), Times.Once);
        }
    }
}
