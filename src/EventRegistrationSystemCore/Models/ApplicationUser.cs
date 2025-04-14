using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Security.Claims;

namespace EventRegistrationSystemCore.Models;

public class ApplicationUser : IdentityUser
{

    public DateTime? LockoutEndDateUtc { get; set; }


    public ApplicationUser()
    {
        Id = Guid.NewGuid().ToString();
    }

    //public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
    //{
    //    // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
    //    var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
    //    // Add custom user claims here
    //    return userIdentity;
    //}
}