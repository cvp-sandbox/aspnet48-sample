using System.Collections.Generic;

namespace EventManagement.Api.Features.Users.Register
{
    public class RegisterResponse
    {
        public bool Success { get; set; }
        public string UserId { get; set; }
        public string Email { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
        public List<string> Errors { get; set; } = new List<string>();
    }
}
