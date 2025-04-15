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