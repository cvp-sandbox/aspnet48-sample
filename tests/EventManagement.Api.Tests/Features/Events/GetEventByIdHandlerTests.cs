using EventManagement.Api.Features.Events.GetEventById;
using EventManagement.Api.Models;
using EventManagement.Api.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace EventManagement.Api.Tests.Features.Events;

public class GetEventByIdHandlerTests
{
    private readonly IEventRepository _eventRepository;
    private readonly IRegistrationRepository _registrationRepository;
    private readonly ILogger<GetEventByIdHandler> _logger;
    private readonly GetEventByIdHandler _handler;

    public GetEventByIdHandlerTests()
    {
        _eventRepository = Substitute.For<IEventRepository>();
        _registrationRepository = Substitute.For<IRegistrationRepository>();
        _logger = Substitute.For<ILogger<GetEventByIdHandler>>();
        _handler = new GetEventByIdHandler(_eventRepository, _registrationRepository, _logger);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnEventWithDetails_WhenEventExists()
    {
        // Arrange
        var eventId = 1;
        var userId = "user1";
        var testEvent = new Event
        {
            EventId = eventId,
            Name = "Test Event",
            Description = "Test Description",
            EventDate = DateTime.Now.AddDays(1),
            Location = "Test Location",
            MaxAttendees = 10,
            CreatedBy = "creator1",
            CreatedDate = DateTime.Now
        };

        _eventRepository.GetEventByIdAsync(eventId).Returns(testEvent);
        _registrationRepository.GetRegistrationAsync(eventId, userId).Returns((Registration)null);
        _eventRepository.GetRegistrationCountAsync(eventId).Returns(5);

        var request = new GetEventByIdRequest(eventId);

        // Act
        var response = await _handler.HandleAsync(request, userId);

        // Assert
        response.Event.Should().BeEquivalentTo(testEvent);
        response.IsRegistered.Should().BeFalse();
        response.IsCreator.Should().BeFalse();
        response.RegistrationCount.Should().Be(5);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnNullEvent_WhenEventDoesNotExist()
    {
        // Arrange
        var eventId = 1;
        var userId = "user1";

        _eventRepository.GetEventByIdAsync(eventId).Returns((Event)null);

        var request = new GetEventByIdRequest(eventId);

        // Act
        var response = await _handler.HandleAsync(request, userId);

        // Assert
        response.Event.Should().BeNull();
        response.IsRegistered.Should().BeFalse();
        response.IsCreator.Should().BeFalse();
        response.RegistrationCount.Should().Be(0);
    }

    [Fact]
    public async Task HandleAsync_ShouldSetIsRegisteredToTrue_WhenUserIsRegistered()
    {
        // Arrange
        var eventId = 1;
        var userId = "user1";
        var testEvent = new Event
        {
            EventId = eventId,
            Name = "Test Event",
            CreatedBy = "creator1"
        };
        var registration = new Registration
        {
            EventId = eventId,
            UserId = userId
        };

        _eventRepository.GetEventByIdAsync(eventId).Returns(testEvent);
        _registrationRepository.GetRegistrationAsync(eventId, userId).Returns(registration);
        _eventRepository.GetRegistrationCountAsync(eventId).Returns(5);

        var request = new GetEventByIdRequest(eventId);

        // Act
        var response = await _handler.HandleAsync(request, userId);

        // Assert
        response.IsRegistered.Should().BeTrue();
    }

    [Fact]
    public async Task HandleAsync_ShouldSetIsCreatorToTrue_WhenUserIsCreator()
    {
        // Arrange
        var eventId = 1;
        var userId = "user1";
        var testEvent = new Event
        {
            EventId = eventId,
            Name = "Test Event",
            CreatedBy = userId
        };

        _eventRepository.GetEventByIdAsync(eventId).Returns(testEvent);
        _registrationRepository.GetRegistrationAsync(eventId, userId).Returns((Registration)null);
        _eventRepository.GetRegistrationCountAsync(eventId).Returns(5);

        var request = new GetEventByIdRequest(eventId);

        // Act
        var response = await _handler.HandleAsync(request, userId);

        // Assert
        response.IsCreator.Should().BeTrue();
    }
}
