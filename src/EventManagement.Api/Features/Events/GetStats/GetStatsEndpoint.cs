using Microsoft.AspNetCore.Mvc;

namespace EventManagement.Api.Features.Events.GetStats;

public static class GetStatsEndpoint
{
    public static WebApplication MapGetStatsEndpoint(this WebApplication app)
    {
        app.MapGet("/api/events/stats", async (
            [FromServices] GetStatsHandler handler) =>
        {
            // No authentication required for stats
            var request = new GetStatsRequest();
            var response = await handler.HandleAsync(request);
            
            return Results.Ok(response);
        })
        .WithName("GetStats")
        .WithOpenApi()
        .WithTags("Events");
        
        return app;
    }
}
