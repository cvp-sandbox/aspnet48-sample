using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace EventManagement.Api.Features.Users.Register
{
    public static class RegisterEndpoint
    {
        public static WebApplication MapRegisterEndpoint(this WebApplication app)
        {
            app.MapPost("/api/users/register", async (RegisterRequest request, RegisterHandler handler) =>
            {
                var response = await handler.HandleAsync(request);
                
                if (!response.Success)
                {
                    return Results.BadRequest(response);
                }
                
                return Results.Created($"/api/users/{response.UserId}", response);
            })
            .WithName("Register")
            .WithOpenApi()
            .AllowAnonymous();
            
            return app;
        }
    }
}
