using System;
using Timelogger.Entities;

namespace Timelogger.Api.DTOs
{
    public class TimeRegistrationDto
    {
        public TimeDuration TimeSpent { get; set; }
        public DateTime RegistrationDate { get; set; }
    }
}
