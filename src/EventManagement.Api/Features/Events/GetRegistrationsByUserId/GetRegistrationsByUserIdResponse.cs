using EventManagement.Api.Models;

namespace EventManagement.Api.Features.Events.GetRegistrationsByUserId;

public record GetRegistrationsByUserIdResponse(IEnumerable<Registration> Registrations);
