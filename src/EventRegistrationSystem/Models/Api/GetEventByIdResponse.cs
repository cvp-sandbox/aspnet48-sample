using System;

namespace EventRegistrationSystem.Models.Api
{
    public class GetEventByIdResponse
    {
        public EventDto Event { get; set; }
        public bool IsRegistered { get; set; }
        public bool IsCreator { get; set; }
        public int RegistrationCount { get; set; }
    }

    // DTO that matches the API's Event model structure
    public class EventDto
    {
        public int EventId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime EventDate { get; set; }
        public string Location { get; set; }
        public int MaxAttendees { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
