using System.ComponentModel.DataAnnotations;

namespace EventManagement.Api.Features.Users.Login
{
    public class LoginRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        
        [Required]
        public string Password { get; set; }
        
        public string ReturnUrl { get; set; }
    }
}
