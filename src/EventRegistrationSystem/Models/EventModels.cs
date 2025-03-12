using System;

namespace EventRegistrationSystem.Models
{
    public class Event
    {
        public int EventId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime EventDate { get; set; }
        public string Location { get; set; }
        public int MaxAttendees { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }

        // Navigation property (not stored in DB)
        public int RegistrationCount { get; set; }
    }

    public class Registration
    {
        public int RegistrationId { get; set; }
        public int EventId { get; set; }
        public string UserId { get; set; }
        public DateTime RegistrationDate { get; set; }

        // Navigation property (not stored in DB)
        public string UserName { get; set; }
        public Event Event { get; set; }
    }
}