

namespace EventRegistrationSystem.Authorization
{
    public class UserRole
    {
        public int UserRoleId { get; set; }
        public string UserId { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }
    }

    public static class Roles
    {
        public const string Admin = "Admin";
        public const string Organizer = "Organizer";
        public const string User = "User";
    }

  
}