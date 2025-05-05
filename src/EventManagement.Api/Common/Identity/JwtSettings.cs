using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace EventManagement.Api.Common.Identity;

public class JwtSettings
{
    public string Issuer { get; set; } = "EventManagementApi";
    public string Audience { get; set; } = "EventManagementSpa";
    public string SecretKey { get; set; } = "YourSuperSecretKeyHere_ThisShouldBeAtLeast32CharsLong!";
    public int ExpirationMinutes { get; set; } = 60;
    
    public SymmetricSecurityKey GetSymmetricSecurityKey()
    {
        return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
    }
}
