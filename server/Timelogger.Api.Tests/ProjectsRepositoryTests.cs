using System.Collections.Generic;
using System;
using Xunit;
using Timelogger.Api.Repositories;
using Timelogger.Entities;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Linq;

namespace Timelogger.Spi.Tests
{
    public class ProjectsRepositoryTests
    {
        private readonly ApiContext _context;
        private readonly ProjectsRepository _repository;

        public ProjectsRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<ApiContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

            _context = new ApiContext(options);
            _repository = new ProjectsRepository(_context);
        }

        [Fact]
        public void GetAllProjects_ReturnsAllProjects()
        {
            // Arrange
            var project = new Project
            {
                Id = 1,
                Name = "Test Project",
                Deadline = DateTime.Now.AddDays(7),
                TimeRegistrations = new List<TimeRegistration>
            {
                new TimeRegistration { 
                    Id = 2, 
                    TimeSpent = TimeDuration.TwoHours, 
                    RegistrationDate = DateTime.Now 
                }
            }
            };

            _context.Projects.Add(project);
            _context.SaveChanges();

            // Act
            var result = _repository.GetAllProjects();

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Test Project", result.First().Name);
            Assert.Single(result.First().TimeRegistrations);
            Assert.Equal(TimeDuration.TwoHours, result.First().TimeRegistrations.First().TimeSpent);
        }

        [Fact]
        public void GetProjectWithTimeRegistrations_ReturnsCorrectProject()
        {
            // Arrange
            var project = new Project
            {
                Id = 2,
                Name = "Test Project",
                Deadline = DateTime.Now.AddDays(7),
                TimeRegistrations = new List<TimeRegistration>
            {
                new TimeRegistration { 
                    Id  = 1, 
                    TimeSpent = TimeDuration.ThirtyMinutes, 
                    RegistrationDate = DateTime.Now 
                }
            }
            };

            _context.Projects.Add(project);
            _context.SaveChanges();

            // Act
            var result = _repository.GetProjectWithTimeRegistrations(project.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test Project", result.Name);
            Assert.Single(result.TimeRegistrations);
            Assert.Equal(TimeDuration.ThirtyMinutes, result.TimeRegistrations.First().TimeSpent);
        }

        [Fact]
        public void AddTimeRegistration_AddsTimeRegistrationToProject()
        {
            // Arrange
            var project = new Project
            {
                Id = 3,
                Name = "Test Project",
                Deadline = DateTime.Now.AddDays(7),
                TimeRegistrations = new List<TimeRegistration>()
            };

            _context.Projects.Add(project);
            _context.SaveChanges();

            var newTimeRegistration = new TimeRegistration { TimeSpent = TimeDuration.ThirtyMinutes, RegistrationDate = DateTime.Now };

            // Act
            _repository.AddTimeRegistration(project.Id, newTimeRegistration);
            _repository.Save();

            // Assert
            var updatedProject = _repository.GetProjectWithTimeRegistrations(project.Id);
            Assert.Single(updatedProject.TimeRegistrations);
            Assert.Equal(TimeDuration.ThirtyMinutes, updatedProject.TimeRegistrations.First().TimeSpent);
        }

        [Fact]
        public void Save_CallsSaveChangesOnContext()
        {
            // Arrange
            var project = new Project { Id = 4, Name = "Test Project" };
            _context.Projects.Add(project);
            _context.SaveChanges();

            // Act
            _repository.Save();

            // Assert
            var savedProject = _context.Projects.Find(project.Id);
            Assert.NotNull(savedProject);
            Assert.Equal("Test Project", savedProject.Name);
        }
    }
}
