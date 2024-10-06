using System.Collections.Generic;
using System;
using Timelogger.Entities;

namespace Timelogger.Api.DTOs
{
    public class ProjectDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime Deadline { get; set; }
        public List<TimeRegistrationDto> TimeRegistrations { get; set; }
        public ProjectStatus Status { get; set; }
    }
}
