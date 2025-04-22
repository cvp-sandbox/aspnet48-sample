using System.Collections.Generic;

namespace EventRegistrationSystem.Models.Api
{
    public class GetEventsByCreatorResponse
    {
        public List<EventDto> Events { get; set; }
    }
}
