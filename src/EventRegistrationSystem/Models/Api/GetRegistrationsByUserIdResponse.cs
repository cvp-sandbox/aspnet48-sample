using System;
using System.Collections.Generic;

namespace EventRegistrationSystem.Models.Api
{
    public class GetRegistrationsByUserIdResponse
    {
        public List<RegistrationDto> Registrations { get; set; }
    }

    public class RegistrationDto
    {
        public int RegistrationId { get; set; }
        public int EventId { get; set; }
        public string UserId { get; set; }
        public DateTime RegistrationDate { get; set; }
        public EventDto Event { get; set; }
    }
}
