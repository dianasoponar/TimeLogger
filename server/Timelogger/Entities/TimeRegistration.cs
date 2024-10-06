using System;

namespace Timelogger.Entities
{
    public class TimeRegistration
    {
        public int Id { get; set; }
        public TimeDuration TimeSpent { get; set; }
        public DateTime RegistrationDate { get; set; }
    }
}