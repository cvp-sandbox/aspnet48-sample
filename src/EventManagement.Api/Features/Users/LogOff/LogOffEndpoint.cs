using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace EventManagement.Api.Features.Users.LogOff
{
    public static class LogOffEndpoint
    {
        public static WebApplication MapLogOffEndpoint(this WebApplication app)
        {
            app.MapPost("/api/users/logoff", async (LogOffHandler handler) =>
            {
                var request = new LogOffRequest();
                var response = await handler.HandleAsync(request);
                
                return Results.Ok(response);
            })
            .WithName("LogOff")
            .WithOpenApi();
            
            return app;
        }
    }
}
