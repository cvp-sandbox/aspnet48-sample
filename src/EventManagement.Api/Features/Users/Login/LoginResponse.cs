using System.Collections.Generic;

namespace EventManagement.Api.Features.Users.Login
{
    public class LoginResponse
    {
        public bool Success { get; set; }
        public string UserId { get; set; }
        public string Email { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
        public string ErrorMessage { get; set; }
        public string ReturnUrl { get; set; }
    }
}
