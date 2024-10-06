using Microsoft.AspNetCore.Mvc;
using System;
using Timelogger.Api.DTOs;
using Timelogger.Api.Repositories;
using Timelogger.Entities;

namespace Timelogger.Api.Controllers
{
	[Route("api/[controller]")]
	public class ProjectsController : Controller
	{
        private readonly IProjectsRepository _projectsRepository;

        public ProjectsController(IProjectsRepository projectsRepository)
		{
            _projectsRepository = projectsRepository;
        }

        // GET api/projects
        [HttpGet]
		public IActionResult Get()
		{
            var projects = _projectsRepository.GetAllProjects();
            return Ok(projects);
        }

        // POST api/projects/{projectId}/time-registration
        [HttpPost("{projectId}/time-registration")]
        public IActionResult AddTimeRegistration(int projectId, [FromBody] TimeRegistrationDto body)
        {
            if (body == null)
            {
                return BadRequest("Invalid time registration data.");
            }

            var project = _projectsRepository.GetProjectWithTimeRegistrations(projectId);
            if (project == null)
            {
                return NotFound("Project not found.");
            }

            var timeRegistration = new TimeRegistration
            {
                TimeSpent = body.TimeSpent,
                RegistrationDate = DateTime.Now.Date
            };

            _projectsRepository.AddTimeRegistration(projectId, timeRegistration);
            _projectsRepository.Save();

            return Ok(project);
        }
    }
}
