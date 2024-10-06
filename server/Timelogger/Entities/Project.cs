using System;
using System.Collections.Generic;

namespace Timelogger.Entities
{
	public class Project
	{
		public int Id { get; set; }
		public string Name { get; set; }
        public DateTime Deadline { get; set; }
        public List<TimeRegistration> TimeRegistrations { get; set; }
        public ProjectStatus Status { 
			get
			{
                if (Deadline > DateTime.Now) return ProjectStatus.InProgress;
                else if (Deadline <= DateTime.Now) return ProjectStatus.Completed;
                else return ProjectStatus.Pending;
            }
		}
    }
}
