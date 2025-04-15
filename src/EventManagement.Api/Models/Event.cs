namespace EventManagement.Api.Models;

public class Event
{
    public int EventId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime EventDate { get; set; }
    public string? Location { get; set; }
    public int MaxAttendees { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
}

public static class Roles{
    public static Role Admin { get;  } = Role.CreateRole(1, "Admin");
    public static Role Organizer { get;  } = Role.CreateRole(2, "Organizer");
    public static Role Attendee { get;  } = Role.CreateRole(3, "User");
    public class Role{
        private Role(int roleId, string name)
        {
            RoleId = roleId;
            Name = name;
        }

        public static Role CreateRole(int roleId, string name)
        {
            return new Role(roleId, name);
        }

        public int RoleId { get; }
        public string Name { get;  }
    }
}