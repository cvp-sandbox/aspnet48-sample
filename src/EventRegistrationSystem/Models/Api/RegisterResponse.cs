using System.Collections.Generic;

namespace EventRegistrationSystem.Models.Api
{
    public class RegisterResponse
    {
        public bool Success { get; set; }
        public string UserId { get; set; }
        public string Email { get; set; }
        public List<string> Roles { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }
}
