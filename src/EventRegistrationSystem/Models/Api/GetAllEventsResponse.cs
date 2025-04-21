using System.Collections.Generic;

namespace EventRegistrationSystem.Models.Api
{
    public class GetAllEventsResponse
    {
        public List<Event> Events { get; set; }
    }
}
