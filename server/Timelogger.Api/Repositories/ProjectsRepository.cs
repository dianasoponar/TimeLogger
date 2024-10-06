using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using Timelogger.Api.DTOs;
using Timelogger.Entities;

namespace Timelogger.Api.Repositories
{
    public class ProjectsRepository : IProjectsRepository
    {
        private readonly ApiContext _context;

        public ProjectsRepository(ApiContext context)
        {
            _context = context;
        }

        // Fetch all projects with their time registrations
        public List<ProjectDto> GetAllProjects()
        {
            return _context.Projects
                .Include(p => p.TimeRegistrations)
                .ToList()
                .Select(p => new ProjectDto
                {
                    Id = p.Id.ToString(),
                    Name = p.Name,
                    Deadline = p.Deadline,
                    TimeRegistrations = p.TimeRegistrations.Select(tr => new TimeRegistrationDto
                    {
                        TimeSpent = tr.TimeSpent,
                        RegistrationDate = tr.RegistrationDate
                    }).ToList(),
                    Status = p.Status
                })
                .ToList();
        }

        // Get a project by id with its time registrations
        public Project GetProjectWithTimeRegistrations(int projectId)
        {
            return _context.Projects
                .Include(p => p.TimeRegistrations)
                .FirstOrDefault(p => p.Id == projectId);
        }

        // Add a time registration to a project
        public void AddTimeRegistration(int projectId, TimeRegistration timeRegistration)
        {
            var project = GetProjectWithTimeRegistrations(projectId);
            if (project != null)
            {
                project.TimeRegistrations.Add(timeRegistration);
            }
        }

        // Save changes to the database
        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
