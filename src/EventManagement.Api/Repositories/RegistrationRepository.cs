using Dapper;
using EventManagement.Api.Features.Events.GetUpcomingEvents;
using EventManagement.Api.Models;
using System.Data;

namespace EventManagement.Api.Repositories;

public class RegistrationRepository : IRegistrationRepository
{
    private readonly IDbConnection _connection;

    public RegistrationRepository(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<IEnumerable<Registration>> GetRegistrationsByEventIdAsync(int eventId)
    {
        var query = @"
            SELECT r.*, u.UserName
            FROM Registrations r
            JOIN AspNetUsers u ON r.UserId = u.Id
            WHERE r.EventId = @EventId
            ORDER BY r.RegistrationDate DESC";
            
        return await _connection.QueryAsync<Registration>(query, new { EventId = eventId });
    }

    public async Task<IEnumerable<Registration>> GetRegistrationsByUserIdAsync(string userId)
    {
        var query = @"
            SELECT r.*, e.*
            FROM Registrations r
            JOIN Events e ON r.EventId = e.EventId
            WHERE r.UserId = @UserId
            ORDER BY e.EventDate DESC";
            
        var registrations = new Dictionary<int, Registration>();
        
        await _connection.QueryAsync<Registration, Event, Registration>(
            query,
            (registration, eventEntity) =>
            {
                if (!registrations.TryGetValue(registration.RegistrationId, out var existingRegistration))
                {
                    existingRegistration = registration;
                    registrations.Add(registration.RegistrationId, existingRegistration);
                }
                
                existingRegistration.Event = eventEntity;
                return existingRegistration;
            },
            new { UserId = userId },
            splitOn: "EventId");
            
        return registrations.Values;
    }

    public async Task<Registration?> GetRegistrationAsync(int eventId, string userId)
    {
        var query = @"
            SELECT * FROM Registrations
            WHERE EventId = @EventId AND UserId = @UserId";
            
        return await _connection.QueryFirstOrDefaultAsync<Registration>(
            query, 
            new { EventId = eventId, UserId = userId });
    }

    public async Task<int> CreateRegistrationAsync(Registration registration)
    {
        var query = @"
            INSERT INTO Registrations (EventId, UserId, RegistrationDate)
            VALUES (@EventId, @UserId, @RegistrationDate);
            SELECT last_insert_rowid();";
            
        return await _connection.ExecuteScalarAsync<int>(query, registration);
    }

    public async Task<bool> CancelRegistrationAsync(int eventId, string userId)
    {
        var query = @"
            DELETE FROM Registrations
            WHERE EventId = @EventId AND UserId = @UserId";
            
        var rowsAffected = await _connection.ExecuteAsync(
            query, 
            new { EventId = eventId, UserId = userId });
            
        return rowsAffected > 0;
    }
    
    public async Task<IEnumerable<RegistrationWithEvent>> GetUpcomingEventsByUsernameAsync(string username)
    {
        var query = @"
            SELECT r.RegistrationId, r.EventId, r.UserId, r.RegistrationDate, u.UserName, e.*
            FROM Registrations r
            JOIN Events e ON r.EventId = e.EventId
            JOIN AspNetUsers u ON u.Id = r.UserId
            WHERE u.UserName = @UserName AND e.EventDate BETWEEN CURRENT_TIMESTAMP AND DATE('now', '+7 days')
            ORDER BY e.EventDate DESC";
            
        var registrations = new Dictionary<int, RegistrationWithEvent>();
        
        await _connection.QueryAsync<RegistrationWithEvent, Event, RegistrationWithEvent>(
            query,
            (registration, eventEntity) =>
            {
                if (!registrations.TryGetValue(registration.RegistrationId, out var existingRegistration))
                {
                    existingRegistration = registration;
                    registrations.Add(registration.RegistrationId, existingRegistration);
                }
                
                existingRegistration.Event = eventEntity;
                return existingRegistration;
            },
            new { UserName = username },
            splitOn: "EventId");
            
        return registrations.Values;
    }
}
