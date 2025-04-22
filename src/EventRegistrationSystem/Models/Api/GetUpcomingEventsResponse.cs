using System.Collections.Generic;
using EventRegistrationSystem.Models;

namespace EventRegistrationSystem.Models.Api
{
    public class RegistrationWithEvent
    {
        public int RegistrationId { get; set; }
        public int EventId { get; set; }
        public string UserId { get; set; }
        public System.DateTime RegistrationDate { get; set; }
        public string UserName { get; set; }
        public Event Event { get; set; }
    }
    
    public class GetUpcomingEventsResponse
    {
        public IEnumerable<RegistrationWithEvent> Registrations { get; set; }
    }
}
