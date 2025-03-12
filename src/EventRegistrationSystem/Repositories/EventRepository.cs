using System;
using System.Collections.Generic;
using Dapper;
using EventRegistrationSystem.Data;
using EventRegistrationSystem.Models;

namespace EventRegistrationSystem.Repositories
{
    public class EventRepository : IEventRepository
    {
        public IEnumerable<Event> GetAllEvents()
        {
            using (var connection = DatabaseConfig.GetConnection())
            {
                connection.Open();
                var events = connection.Query<Event>(@"
                    SELECT e.*, 
                    (SELECT COUNT(*) FROM Registrations r WHERE r.EventId = e.EventId) AS RegistrationCount
                    FROM Events e
                    ORDER BY e.EventDate DESC");
                return events;
            }
        }

        public Event GetEventById(int eventId)
        {
            using (var connection = DatabaseConfig.GetConnection())
            {
                connection.Open();
                var eventEntity = connection.QueryFirstOrDefault<Event>(@"
                    SELECT e.*, 
                    (SELECT COUNT(*) FROM Registrations r WHERE r.EventId = e.EventId) AS RegistrationCount
                    FROM Events e
                    WHERE e.EventId = @EventId", new { EventId = eventId });
                return eventEntity;
            }
        }

        public IEnumerable<Event> GetEventsByCreator(string userId)
        {
            using (var connection = DatabaseConfig.GetConnection())
            {
                connection.Open();
                var events = connection.Query<Event>(@"
                    SELECT e.*, 
                    (SELECT COUNT(*) FROM Registrations r WHERE r.EventId = e.EventId) AS RegistrationCount
                    FROM Events e
                    WHERE e.CreatedBy = @UserId
                    ORDER BY e.EventDate DESC", new { UserId = userId });
                return events;
            }
        }

        public int CreateEvent(Event eventEntity)
        {
            using (var connection = DatabaseConfig.GetConnection())
            {
                connection.Open();
                var sql = @"
                    INSERT INTO Events (Name, Description, EventDate, Location, MaxAttendees, CreatedBy, CreatedDate)
                    VALUES (@Name, @Description, @EventDate, @Location, @MaxAttendees, @CreatedBy, @CreatedDate);
                    SELECT last_insert_rowid();";

                return connection.ExecuteScalar<int>(sql, new
                {
                    eventEntity.Name,
                    eventEntity.Description,
                    EventDate = eventEntity.EventDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    eventEntity.Location,
                    eventEntity.MaxAttendees,
                    eventEntity.CreatedBy,
                    CreatedDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                });
            }
        }

        public bool UpdateEvent(Event eventEntity)
        {
            using (var connection = DatabaseConfig.GetConnection())
            {
                connection.Open();
                var sql = @"
                    UPDATE Events
                    SET Name = @Name, 
                        Description = @Description, 
                        EventDate = @EventDate, 
                        Location = @Location, 
                        MaxAttendees = @MaxAttendees
                    WHERE EventId = @EventId";

                int rowsAffected = connection.Execute(sql, new
                {
                    eventEntity.EventId,
                    eventEntity.Name,
                    eventEntity.Description,
                    EventDate = eventEntity.EventDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    eventEntity.Location,
                    eventEntity.MaxAttendees
                });

                return rowsAffected > 0;
            }
        }

        public bool DeleteEvent(int eventId)
        {
            using (var connection = DatabaseConfig.GetConnection())
            {
                connection.Open();

                // Delete registrations first
                connection.Execute("DELETE FROM Registrations WHERE EventId = @EventId", new { EventId = eventId });

                // Then delete the event
                int rowsAffected = connection.Execute("DELETE FROM Events WHERE EventId = @EventId", new { EventId = eventId });

                return rowsAffected > 0;
            }
        }

        public int GetRegistrationCount(int eventId)
        {
            using (var connection = DatabaseConfig.GetConnection())
            {
                connection.Open();
                return connection.ExecuteScalar<int>(
                    "SELECT COUNT(*) FROM Registrations WHERE EventId = @EventId",
                    new { EventId = eventId });
            }
        }
    }
}