using System.Collections.Generic;

namespace EventRegistrationSystem.Models.Api
{
    public class LoginResponse
    {
        public bool Success { get; set; }
        public string UserId { get; set; }
        public string Email { get; set; }
        public List<string> Roles { get; set; }
        public string ErrorMessage { get; set; }
        public string ReturnUrl { get; set; }
    }
}
