using EventManagement.Api.Features.Events.RegisterForEvent;
using EventManagement.Api.Models;
using EventManagement.Api.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace EventManagement.Api.Tests.Features.Events;

public class RegisterForEventHandlerTests
{
    private readonly IEventRepository _eventRepository;
    private readonly IRegistrationRepository _registrationRepository;
    private readonly ILogger<RegisterForEventHandler> _logger;
    private readonly RegisterForEventHandler _handler;

    public RegisterForEventHandlerTests()
    {
        _eventRepository = Substitute.For<IEventRepository>();
        _registrationRepository = Substitute.For<IRegistrationRepository>();
        _logger = Substitute.For<ILogger<RegisterForEventHandler>>();
        _handler = new RegisterForEventHandler(_eventRepository, _registrationRepository, _logger);
    }

    [Fact]
    public async Task HandleAsync_ShouldRegisterUserForEvent_WhenEventExistsAndNotFull()
    {
        // Arrange
        var eventId = 1;
        var userId = "user1";
        var registrationId = 5;
        var testEvent = new Event
        {
            EventId = eventId,
            Name = "Test Event",
            MaxAttendees = 10
        };

        _eventRepository.GetEventByIdAsync(eventId).Returns(testEvent);
        _registrationRepository.GetRegistrationAsync(eventId, userId).Returns((Registration)null);
        _eventRepository.GetRegistrationCountAsync(eventId).Returns(5);
        _registrationRepository.CreateRegistrationAsync(Arg.Any<Registration>()).Returns(registrationId);

        var request = new RegisterForEventRequest(eventId);

        // Act
        var response = await _handler.HandleAsync(request, userId);

        // Assert
        response.Success.Should().BeTrue();
        response.Message.Should().Contain("successfully registered");
        response.RegistrationId.Should().Be(registrationId);

        // Verify registration was created with correct properties
        await _registrationRepository.Received(1).CreateRegistrationAsync(Arg.Is<Registration>(r => 
            r.EventId == eventId &&
            r.UserId == userId));
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnFailure_WhenEventDoesNotExist()
    {
        // Arrange
        var eventId = 1;
        var userId = "user1";

        _eventRepository.GetEventByIdAsync(eventId).Returns((Event)null);

        var request = new RegisterForEventRequest(eventId);

        // Act
        var response = await _handler.HandleAsync(request, userId);

        // Assert
        response.Success.Should().BeFalse();
        response.Message.Should().Contain("not found");
        response.RegistrationId.Should().BeNull();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnFailure_WhenUserAlreadyRegistered()
    {
        // Arrange
        var eventId = 1;
        var userId = "user1";
        var testEvent = new Event { EventId = eventId, Name = "Test Event" };
        var existingRegistration = new Registration { EventId = eventId, UserId = userId };

        _eventRepository.GetEventByIdAsync(eventId).Returns(testEvent);
        _registrationRepository.GetRegistrationAsync(eventId, userId).Returns(existingRegistration);

        var request = new RegisterForEventRequest(eventId);

        // Act
        var response = await _handler.HandleAsync(request, userId);

        // Assert
        response.Success.Should().BeFalse();
        response.Message.Should().Contain("already registered");
        response.RegistrationId.Should().BeNull();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnFailure_WhenEventIsFull()
    {
        // Arrange
        var eventId = 1;
        var userId = "user1";
        var testEvent = new Event
        {
            EventId = eventId,
            Name = "Test Event",
            MaxAttendees = 10
        };

        _eventRepository.GetEventByIdAsync(eventId).Returns(testEvent);
        _registrationRepository.GetRegistrationAsync(eventId, userId).Returns((Registration)null);
        _eventRepository.GetRegistrationCountAsync(eventId).Returns(10); // Event is full

        var request = new RegisterForEventRequest(eventId);

        // Act
        var response = await _handler.HandleAsync(request, userId);

        // Assert
        response.Success.Should().BeFalse();
        response.Message.Should().Contain("full capacity");
        response.RegistrationId.Should().BeNull();
    }

    [Fact]
    public async Task HandleAsync_ShouldLogError_WhenExceptionOccurs()
    {
        // Arrange
        var eventId = 1;
        var userId = "user1";
        var testEvent = new Event { EventId = eventId, Name = "Test Event" };

        _eventRepository.GetEventByIdAsync(eventId).Returns(testEvent);
        _registrationRepository.GetRegistrationAsync(eventId, userId).Returns((Registration)null);
        _eventRepository.GetRegistrationCountAsync(eventId).Returns(5);

        var expectedException = new Exception("Test exception");
        _registrationRepository.When(x => x.CreateRegistrationAsync(Arg.Any<Registration>()))
            .Do(x => { throw expectedException; });

        var request = new RegisterForEventRequest(eventId);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _handler.HandleAsync(request, userId));
        
        // Verify error was logged
        _logger.Received(1).Log(
            LogLevel.Error,
            Arg.Any<EventId>(),
            Arg.Any<object>(),
            expectedException,
            Arg.Any<Func<object, Exception, string>>());
    }
}
