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