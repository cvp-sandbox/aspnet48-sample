using Microsoft.Extensions.Primitives;
using System.Security.Claims;

namespace EventManagement.Api.Common.DependencyInjection;

// Create a middleware class for handling header-based authentication
public class HeaderAuthenticationMiddleware
{
    private readonly RequestDelegate _next;

    public HeaderAuthenticationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Check if we have the username header
        if (context.Request.Headers.TryGetValue("X-Username", out var usernameValues) &&
            !string.IsNullOrEmpty(usernameValues))
        {
            var username = usernameValues.ToString();

            // Create claims for the user
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.NameIdentifier, username)
            };

            // Add role claims if present
            if (context.Request.Headers.TryGetValue("X-Role", out var roleValues))
            {
                foreach (var roleValue in roleValues)
                {
                    // Split in case we received comma-separated values
                    var splitRoles = roleValue.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    foreach (var role in splitRoles)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, role.Trim()));
                    }
                }
            }



            // Create the identity and principal
            var identity = new ClaimsIdentity(claims, "Header");
            var principal = new ClaimsPrincipal(identity);

            // Set the user on the HttpContext
            context.User = principal;
        }

        // Continue processing the request
        await _next(context);
    }
}

// Extension method to add the middleware to the pipeline
public static class HeaderAuthenticationMiddlewareExtensions
{
    public static IApplicationBuilder UseHeaderAuthentication(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<HeaderAuthenticationMiddleware>();
    }
}
