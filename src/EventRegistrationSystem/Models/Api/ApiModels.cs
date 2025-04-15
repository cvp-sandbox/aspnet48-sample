using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EventRegistrationSystem.Models.Api
{
    public class GetAllEventsResponse {
        public IEnumerable<Event> Events { get; set; } = Enumerable.Empty<Event>();
    };
}