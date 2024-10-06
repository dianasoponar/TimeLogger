using System.Collections.Generic;
using Timelogger.Api.DTOs;
using Timelogger.Entities;

namespace Timelogger.Api.Repositories
{
    public interface IProjectsRepository
    {
        List<ProjectDto> GetAllProjects();
        Project GetProjectWithTimeRegistrations(int projectId);
        void AddTimeRegistration(int projectId, TimeRegistration timeRegistration);
        void Save();
    }
}
