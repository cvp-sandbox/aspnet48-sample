using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace EventManagement.Api.Features.Users.Login
{
    public static class LoginEndpoint
    {
        public static WebApplication MapLoginEndpoint(this WebApplication app)
        {
            app.MapPost("/api/users/login", async (LoginRequest request, LoginHandler handler) =>
            {
                var response = await handler.HandleAsync(request);
                
                // Log the response for debugging
                Console.WriteLine($"Login response: Success={response.Success}, Email={response.Email}, Roles={string.Join(",", response.Roles)}, Token={response.Token?.Substring(0, 20)}...");
                
                if (!response.Success)
                {
                    return Results.BadRequest(response);
                }
                
                return Results.Ok(response);
            })
            .WithName("Login")
            .WithOpenApi()
            .AllowAnonymous();
            
            return app;
        }
    }
}
