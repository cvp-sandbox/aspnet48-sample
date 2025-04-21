using EventManagement.Api.Features.Events.GetAllEvents;
using EventManagement.Api.Features.Events.GetEventById;
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
                var registrationRepository = Substitute.For<IRegistrationRepository>();
                
                // Setup mock data
                var events = new List<Event>
                {
                    new Event
                    {
                        EventId = 1,
                        Name = "Test Event 1",
                        EventDate = DateTime.Now.AddDays(1),
                        CreatedBy = "user1",
                        CreatedDate = DateTime.Now
                    },
                    new Event
                    {
                        EventId = 2,
                        Name = "Test Event 2",
                        EventDate = DateTime.Now.AddDays(2),
                        CreatedBy = "user2",
                        CreatedDate = DateTime.Now
                    }
                };
                
                eventRepository.GetAllEventsAsync().Returns(events);
                eventRepository.GetEventByIdAsync(1).Returns(events[0]);
                eventRepository.GetEventByIdAsync(2).Returns(events[1]);
                eventRepository.GetRegistrationCountAsync(1).Returns(5);
                eventRepository.GetRegistrationCountAsync(2).Returns(10);
                
                // Replace the real repositories with the mocks
                services.AddScoped<IEventRepository>(_ => eventRepository);
                services.AddScoped<IRegistrationRepository>(_ => registrationRepository);
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
        Assert.Equal(2, result.Events.Count());
        Assert.Equal("Test Event 1", result.Events.First().Name);
    }
    
    [Fact]
    public async Task GetEventById_ReturnsSuccessAndEvent_WhenEventExists()
    {
        // Arrange
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Username", "testuser");

        // Act
        var response = await client.GetAsync("/api/events/1");

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<GetEventByIdResponse>(content, 
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        
        Assert.NotNull(result);
        Assert.NotNull(result.Event);
        Assert.Equal(1, result.Event.EventId);
        Assert.Equal("Test Event 1", result.Event.Name);
        Assert.Equal(5, result.RegistrationCount);
    }
    
    [Fact]
    public async Task GetEventById_ReturnsNotFound_WhenEventDoesNotExist()
    {
        // Arrange
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Username", "testuser");

        // Act
        var response = await client.GetAsync("/api/events/999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
