namespace EventManagement.Api.Models;

public class Registration
{
    public int RegistrationId { get; set; }
    public int EventId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public DateTime RegistrationDate { get; set; }
    
    // Navigation properties
    public string? UserName { get; set; }
    public Event? Event { get; set; }
}
