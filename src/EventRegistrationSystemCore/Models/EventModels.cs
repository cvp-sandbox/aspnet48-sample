using System;

namespace EventRegistrationSystemCore.Models
{
    public class Event
    {
        public int EventId { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public DateTime EventDate { get; set; }
        public required string Location { get; set; }
        public int MaxAttendees { get; set; }
        public required string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }

        // Navigation property (not stored in DB)
        public int RegistrationCount { get; set; }
    }

    public class Registration
    {
        public int RegistrationId { get; set; }
        public int EventId { get; set; }
        public required string UserId { get; set; }
        public DateTime RegistrationDate { get; set; }

        // Navigation property (not stored in DB)
        public required string UserName { get; set; }
        public required Event Event { get; set; }
    }
}
