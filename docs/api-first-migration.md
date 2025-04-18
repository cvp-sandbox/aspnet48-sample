# Step-by-Step Guide: Migrating from .NET Framework 4.8 MVC to .NET 9.0 Minimal API with Aspire

This guide will walk you through setting up a .NET 9.0 Web Minimal API project and migrating your first endpoint from the legacy ASP.NET MVC application. We'll implement the REPR pattern, set up unit testing, and configure Aspire for development orchestration.

## Step 1: Set Up Solution Structure

1. Create a new solution folder structure:

```
EventManagementSystem/
├── src/
│   ├── EventManagement.Legacy/         (Existing MVC project)
│   ├── EventManagement.Api/            (New .NET 9.0 Minimal API)
│   └── EventManagement.Aspire.AppHost/ (Aspire host project)
└── test/
    └── EventManagement.Api.Tests/      (xUnit test project)
```

2. Open a command prompt and navigate to your solution directory:

```bash
mkdir test
```

## Step 2: Create the .NET 9.0 Minimal API Project

```bash
cd src
dotnet new web -n EventManagement.Api -f net9.0
```

## Step 3: Create the xUnit Test Project

```bash
cd ../test
dotnet new xunit -n EventManagement.Api.Tests -f net9.0
cd EventManagement.Api.Tests
dotnet add package NSubstitute
dotnet add package FluentAssertions
dotnet add package Microsoft.AspNetCore.Mvc.Testing
dotnet add reference ../../src/EventManagement.Api/EventManagement.Api.csproj
```

## Step 4: Set Up Aspire Projects

```bash
cd ../../src
dotnet new aspire-apphost -n EventManagement.Aspire.AppHost -f net9.0
dotnet new aspire-servicedefaults -n EventManagement.Aspire.ServiceDefaults -f net9.0
```

## Step 5: Add Projects to Aspire AppHost

```bash
cd ../src/EventManagement.Aspire.AppHost
dotnet add reference ../src/EventManagement.Api/EventManagement.Api.csproj
dotnet add reference ../src/EventRegistrationSystem/EventRegistrationSystem.csproj
```

Edit the `Program.cs` in the `EventManagement.Aspire.AppHost` project:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.EventManagement_Api>("api");

var legacyApp = builder.AddProject<Projects.EventRegistrationSystem>("legacy-mvc")
                .WithEndpoint("http", endpoint => {

                    endpoint.Port = 44372; // Match HTTP port
                    endpoint.IsProxied = false; // Bypass Aspire's reverse proxy if needed
                    endpoint.UriScheme = "https";
                });

builder.Build().Run();
```

## Step 6: Update Project References

```bash
cd EventManagement.Aspire.AppHost
dotnet add reference ../EventManagement.Api/EventManagement.Api.csproj

cd ../EventManagement.Api
dotnet add reference ../EventManagement.Aspire.ServiceDefaults/EventManagement.Aspire.ServiceDefaults.csproj
dotnet add package Dapper
dotnet add package Microsoft.Data.Sqlite
```

## Step 7: Set Up the API Project Structure with REPR Pattern

Create the following folder structure in the `EventManagement.Api` project:

```
EventManagement.Api/
├── Common/
│   └── DependencyInjection/
│       └── DbConnectionExtensions.cs
├── Features/
│   └── Events/
│       ├── GetAllEvents/
│       │   ├── GetAllEventsEndpoint.cs
│       │   ├── GetAllEventsRequest.cs
│       │   ├── GetAllEventsResponse.cs
│       │   └── GetAllEventsHandler.cs
│       └── EventsModule.cs
├── Models/
│   └── Event.cs
├── Repositories/
│   ├── EventRepository.cs
│   └── IEventRepository.cs
├── Configuration/
│   └── DotEnvReader.cs
├── .env
├── appsettings.json
└── Program.cs
```

## Step 8: Implement Database Connection Configuration

Create the `DbConnectionExtensions.cs` file:

```csharp
using Microsoft.Data.Sqlite;
using System.Data;

namespace EventManagement.Api.Common.DependencyInjection;

public static class DbConnectionExtensions
{
    public static IServiceCollection AddSqliteConnection(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IDbConnection>(sp =>
        {
            var dbPath = configuration["DatabasePath"];
            var connectionString = $"Data Source={dbPath};Version=3;";
            return new SqliteConnection(connectionString);
        });
        
        return services;
    }
}
```

## Step 9: Implement Environment File Reader

Create the `DotEnvReader.cs` file:

```csharp
namespace EventManagement.Api.Configuration;

public static class DotEnvReader
{
    public static void Load(string filePath)
    {
        if (!File.Exists(filePath))
            return;
            
        foreach (var line in File.ReadAllLines(filePath))
        {
            var parts = line.Split('=', 2);
            if (parts.Length != 2)
                continue;
                
            var key = parts[0].Trim();
            var value = parts[1].Trim();
            
            if (string.IsNullOrEmpty(key))
                continue;
                
            Environment.SetEnvironmentVariable(key, value);
        }
    }
}
```

## Step 10: Create the Event Model

Create the `Event.cs` file:

```csharp
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
```

## Step 11: Implement Event Repository

Create the `IEventRepository.cs` interface:

```csharp
using EventManagement.Api.Models;

namespace EventManagement.Api.Repositories;

public interface IEventRepository
{
    Task<IEnumerable<Event>> GetAllEventsAsync();
    Task<Event?> GetEventByIdAsync(int eventId);
    Task<IEnumerable<Event>> GetEventsByCreatorAsync(string userId);
    Task<int> CreateEventAsync(Event eventEntity);
    Task<bool> UpdateEventAsync(Event eventEntity);
    Task<bool> DeleteEventAsync(int eventId);
    Task<int> GetRegistrationCountAsync(int eventId);
}
```

Create the `EventRepository.cs` implementation:

```csharp
using Dapper;
using EventManagement.Api.Models;
using System.Data;

namespace EventManagement.Api.Repositories;

public class EventRepository : IEventRepository
{
    private readonly IDbConnection _connection;

    public EventRepository(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<IEnumerable<Event>> GetAllEventsAsync()
    {
        var query = "SELECT * FROM Events ORDER BY EventDate";
        return await _connection.QueryAsync<Event>(query);
    }

    public async Task<Event?> GetEventByIdAsync(int eventId)
    {
        var query = "SELECT * FROM Events WHERE EventId = @EventId";
        return await _connection.QueryFirstOrDefaultAsync<Event>(query, new { EventId = eventId });
    }

    public async Task<IEnumerable<Event>> GetEventsByCreatorAsync(string userId)
    {
        var query = "SELECT * FROM Events WHERE CreatedBy = @UserId ORDER BY EventDate";
        return await _connection.QueryAsync<Event>(query, new { UserId = userId });
    }

    public async Task<int> CreateEventAsync(Event eventEntity)
    {
        var query = @"
            INSERT INTO Events (Name, Description, EventDate, Location, MaxAttendees, CreatedBy, CreatedDate)
            VALUES (@Name, @Description, @EventDate, @Location, @MaxAttendees, @CreatedBy, @CreatedDate);
            SELECT last_insert_rowid();";
            
        return await _connection.ExecuteScalarAsync<int>(query, eventEntity);
    }

    public async Task<bool> UpdateEventAsync(Event eventEntity)
    {
        var query = @"
            UPDATE Events 
            SET Name = @Name, 
                Description = @Description, 
                EventDate = @EventDate, 
                Location = @Location, 
                MaxAttendees = @MaxAttendees
            WHERE EventId = @EventId";
            
        var rowsAffected = await _connection.ExecuteAsync(query, eventEntity);
        return rowsAffected > 0;
    }

    public async Task<bool> DeleteEventAsync(int eventId)
    {
        var query = "DELETE FROM Events WHERE EventId = @EventId";
        var rowsAffected = await _connection.ExecuteAsync(query, new { EventId = eventId });
        return rowsAffected > 0;
    }

    public async Task<int> GetRegistrationCountAsync(int eventId)
    {
        var query = "SELECT COUNT(*) FROM Registrations WHERE EventId = @EventId";
        return await _connection.ExecuteScalarAsync<int>(query, new { EventId = eventId });
    }
}
```

## Step 12: Implement the REPR Pattern for GetAllEvents

Create the `GetAllEventsRequest.cs` file:

```csharp
namespace EventManagement.Api.Features.Events.GetAllEvents;

public record GetAllEventsRequest();
```

Create the `GetAllEventsResponse.cs` file:

```csharp
using EventManagement.Api.Models;

namespace EventManagement.Api.Features.Events.GetAllEvents;

public record GetAllEventsResponse(IEnumerable<Event> Events);
```

Create the `GetAllEventsHandler.cs` file:

```csharp
using EventManagement.Api.Repositories;

namespace EventManagement.Api.Features.Events.GetAllEvents;

public class GetAllEventsHandler
{
    private readonly IEventRepository _eventRepository;
    private readonly ILogger<GetAllEventsHandler> _logger;

    public GetAllEventsHandler(IEventRepository eventRepository, ILogger<GetAllEventsHandler> logger)
    {
        _eventRepository = eventRepository;
        _logger = logger;
    }

    public async Task<GetAllEventsResponse> HandleAsync(GetAllEventsRequest request)
    {
        _logger.LogInformation("Retrieving all events");
        
        try
        {
            var events = await _eventRepository.GetAllEventsAsync();
            return new GetAllEventsResponse(events);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all events");
            throw;
        }
    }
}
```

Create the `GetAllEventsEndpoint.cs` file:

```csharp
using Microsoft.AspNetCore.Mvc;

namespace EventManagement.Api.Features.Events.GetAllEvents;

public static class GetAllEventsEndpoint
{
    public static WebApplication MapGetAllEventsEndpoint(this WebApplication app)
    {
        app.MapGet("/api/events", async (
            [FromServices] GetAllEventsHandler handler,
            [FromHeader(Name = "X-Username")] string? username) =>
        {
            // You can use the username from header for authentication validation if needed
            
            var request = new GetAllEventsRequest();
            var response = await handler.HandleAsync(request);
            
            return Results.Ok(response);
        })
        .WithName("GetAllEvents")
        .WithOpenApi()
        .WithTags("Events");
        
        return app;
    }
}
```

## Step 13: Create the Events Module

Create the `EventsModule.cs` file:

```csharp
using EventManagement.Api.Features.Events.GetAllEvents;
using EventManagement.Api.Repositories;

namespace EventManagement.Api.Features.Events;

public static class EventsModule
{
    public static IServiceCollection AddEventsModule(this IServiceCollection services)
    {
        // Register repositories
        services.AddScoped<IEventRepository, EventRepository>();
        
        // Register handlers
        services.AddScoped<GetAllEventsHandler>();
        
        return services;
    }
    
    public static WebApplication MapEventsEndpoints(this WebApplication app)
    {
        app.MapGetAllEventsEndpoint();
        
        return app;
    }
}
```

## Step 14: Update Program.cs

Update the `Program.cs` file:

```csharp
using EventManagement.Api.Common.DependencyInjection;
using EventManagement.Api.Configuration;
using EventManagement.Api.Features.Events;

// Load environment variables from .env file
DotEnvReader.Load(Path.Combine(Directory.GetCurrentDirectory(), ".env"));

var builder = WebApplication.CreateBuilder(args);

// Add service defaults (telemetry, health checks, etc.)
builder.AddServiceDefaults();

// Add configuration from environment variables after loading from appsettings
builder.Configuration.AddEnvironmentVariables();

// Add services to the container
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add SQLite connection
builder.Services.AddSqliteConnection(builder.Configuration);

// Add feature modules
builder.Services.AddEventsModule();

var app = builder.Build();

// Configure middleware
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

// Map endpoints
app.MapEventsEndpoints();

// Map service defaults (health checks, etc.)
app.MapDefaultEndpoints();

app.Run();

// Make Program class accessible to tests
public partial class Program { }
```

## Step 15: Create the .env file

Create a `.env` file in the API project:

```
DatabasePath=../EventManagement.Legacy/App_Data/EventManagement.db
```

## Step 16: Create Unit Tests

Create a test class for the `GetAllEventsHandler`:

```csharp
using EventManagement.Api.Features.Events.GetAllEvents;
using EventManagement.Api.Models;
using EventManagement.Api.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace EventManagement.Api.Tests.Features.Events;

public class GetAllEventsHandlerTests
{
    private readonly IEventRepository _eventRepository;
    private readonly ILogger<GetAllEventsHandler> _logger;
    private readonly GetAllEventsHandler _handler;

    public GetAllEventsHandlerTests()
    {
        _eventRepository = Substitute.For<IEventRepository>();
        _logger = Substitute.For<ILogger<GetAllEventsHandler>>();
        _handler = new GetAllEventsHandler(_eventRepository, _logger);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnAllEvents()
    {
        // Arrange
        var events = new List<Event>
        {
            new Event
            {
                EventId = 1,
                Name = "Test Event 1",
                Description = "Description 1",
                EventDate = DateTime.Now.AddDays(1),
                Location = "Location 1",
                MaxAttendees = 10,
                CreatedBy = "user1",
                CreatedDate = DateTime.Now
            },
            new Event
            {
                EventId = 2,
                Name = "Test Event 2",
                Description = "Description 2",
                EventDate = DateTime.Now.AddDays(2),
                Location = "Location 2",
                MaxAttendees = 20,
                CreatedBy = "user2",
                CreatedDate = DateTime.Now
            }
        };

        _eventRepository.GetAllEventsAsync().Returns(events);

        // Act
        var response = await _handler.HandleAsync(new GetAllEventsRequest());

        // Assert
        response.Events.Should().BeEquivalentTo(events);
        await _eventRepository.Received(1).GetAllEventsAsync();
    }

    [Fact]
    public async Task HandleAsync_WhenRepositoryThrowsException_ShouldRethrowException()
    {
        // Arrange
        _eventRepository.GetAllEventsAsync().Returns(_ => throw new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _handler.HandleAsync(new GetAllEventsRequest()));
    }
}
```

Create an integration test class:

```csharp
using EventManagement.Api.Features.Events.GetAllEvents;
using EventManagement.Api.Models;
using EventManagement.Api.Repositories;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using System.Net;
using System.Text.Json;
using Xunit;

namespace EventManagement.Api.Tests.Integration;

public class EventsApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public EventsApiTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Mock the event repository for integration tests
                var eventRepository = Substitute.For<IEventRepository>();
                var events = new List<Event>
                {
                    new Event
                    {
                        EventId = 1,
                        Name = "Test Event 1",
                        EventDate = DateTime.Now.AddDays(1),
                        CreatedBy = "user1",
                        CreatedDate = DateTime.Now
                    }
                };
                
                eventRepository.GetAllEventsAsync().Returns(events);
                
                // Replace the real repository with the mock
                services.AddScoped<IEventRepository>(_ => eventRepository);
            });
        });
    }

    [Fact]
    public async Task GetAllEvents_ReturnsSuccessAndEvents()
    {
        // Arrange
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Username", "testuser");

        // Act
        var response = await client.GetAsync("/api/events");

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<GetAllEventsResponse>(content, 
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        
        Assert.NotNull(result);
        Assert.Single(result.Events);
        Assert.Equal("Test Event 1", result.Events.First().Name);
    }
}
```

## Step 17: Modify the Legacy MVC Controller

After setting up the new API, you'll need to modify the legacy EventController to call the API:

```csharp
public class EventController : Controller
{
    private readonly HttpClient _httpClient;
    private readonly string _apiBaseUrl;

    public EventController()
    {
        _httpClient = new HttpClient();
        _apiBaseUrl = ConfigurationManager.AppSettings["ApiBaseUrl"] ?? "https://localhost:5001";
    }

    [HttpGet]
    public async Task<ActionResult> Index()
    {
        try
        {
            // Get current user info
            var username = User.Identity.GetUserName();
            
            // Set headers for the API call
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("X-Username", username);
            
            // Add roles if needed
            if (User.IsInRole(Roles.Admin))
            {
                _httpClient.DefaultRequestHeaders.Add("X-Role", Roles.Admin);
            }
            else if (User.IsInRole(Roles.Organizer))
            {
                _httpClient.DefaultRequestHeaders.Add("X-Role", Roles.Organizer);
            }
            else
            {
                _httpClient.DefaultRequestHeaders.Add("X-Role", Roles.User);
            }

            // Call the API
            var response = await _httpClient.GetAsync($"{_apiBaseUrl}/api/events");
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<dynamic>(content);
                var events = JsonConvert.DeserializeObject<List<Event>>(result.events.ToString());
                
                return View(events);
            }
            
            return View("Error");
        }
        catch (Exception ex)
        {
            // Log the exception
            return View("Error");
        }
    }
    
    // Other actions...
}
```

## Step 18: Run and Test the Projects with Aspire

1. Add a `.env` file to the AppHost project:

```
DatabasePath=../EventManagement.Legacy/App_Data/EventManagement.db
```

2. Navigate to the AppHost directory and run the Aspire orchestration:

```bash
cd ../EventManagement.Aspire.AppHost
dotnet run
```

This will start the Aspire dashboard and run your API project.

## Key Points to Remember

1. **Database Configuration:**
   - Both applications share the same SQLite database
   - The database path is configured via the `.env` file

2. **Authentication/Authorization:**
   - The legacy app passes user information via HTTP headers
   - The API can validate user permissions based on these headers

3. **REPR Pattern:**
   - Request objects define the input
   - Endpoints map the HTTP routes
   - Handler classes contain the business logic
   - Response objects define the output

4. **Testing:**
   - Unit tests focus on handler logic using NSubstitute for mocking
   - Integration tests use WebApplicationFactory to test API endpoints

5. **Dependency Injection:**
   - SQLite connections are registered as scoped services
   - Repositories and handlers are registered appropriately

6. **Aspire Configuration:**
   - Orchestrates the API project (and eventually the legacy app)
   - Manages environment variables and configuration

By following these steps, you've successfully migrated your first endpoint to a .NET 9.0 Minimal API while maintaining compatibility with your legacy application. You can continue this process for other endpoints, gradually moving the business logic from the MVC controllers to the new API.