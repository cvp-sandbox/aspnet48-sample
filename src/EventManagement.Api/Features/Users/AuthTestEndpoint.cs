using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventManagement.Api.Features.Users;

public static class AuthTestEndpoint
{
    public static void MapAuthTestEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/auth/test", [Authorize] (HttpContext context) =>
        {
            // Get information about the authenticated user
            var user = context.User;
            var identity = user.Identity;
            
            // Determine which authentication mechanism was used
            string authMechanism = identity?.AuthenticationType ?? "Unknown";
            
            // Get the username
            var username = user.Identity?.Name ?? "Unknown";
            
            // Get the roles
            var roles = user.Claims
                .Where(c => c.Type == System.Security.Claims.ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList();
            
            // Return the authentication information
            return Results.Ok(new
            {
                IsAuthenticated = identity?.IsAuthenticated ?? false,
                AuthenticationType = authMechanism,
                Username = username,
                Roles = roles,
                AllClaims = user.Claims.Select(c => new { Type = c.Type, Value = c.Value })
            });
        })
        .WithName("AuthTest")
        .WithOpenApi();
    }
}
