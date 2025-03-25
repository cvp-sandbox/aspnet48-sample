using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventManagementSystemv2.Models
{
    public class Event
    {
        public int EventId { get; set; }
        
        [Required]
        public string Name { get; set; } = string.Empty;
        
        public string Description { get; set; } = string.Empty;
        
        [Required]
        public DateTime EventDate { get; set; }
        
        public string Location { get; set; } = string.Empty;
        
        public int MaxAttendees { get; set; }
        
        public string CreatedBy { get; set; } = string.Empty;
        
        public DateTime CreatedDate { get; set; }
        
        // Navigation property
        [NotMapped]
        public int RegistrationCount { get; set; }
    }

    public class Registration
    {
        public int RegistrationId { get; set; }
        
        public int EventId { get; set; }
        
        public string UserId { get; set; } = string.Empty;
        
        public DateTime RegistrationDate { get; set; }

        // Navigation properties
        [NotMapped]
        public string UserName { get; set; } = string.Empty;
        
        [NotMapped]
        public Event? Event { get; set; }
    }
}
