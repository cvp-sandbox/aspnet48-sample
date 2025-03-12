using System;
using System.Collections.Generic;
using Dapper;
using EventRegistrationSystem.Data;
using EventRegistrationSystem.Models;

namespace EventRegistrationSystem.Repositories
{
    public class RegistrationRepository : IRegistrationRepository
    {
        public IEnumerable<Registration> GetRegistrationsByEventId(int eventId)
        {
            using (var connection = DatabaseConfig.GetConnection())
            {
                connection.Open();
                return connection.Query<Registration>(@"
                    SELECT r.*, u.UserName
                    FROM Registrations r
                    JOIN AspNetUsers u ON r.UserId = u.Id
                    WHERE r.EventId = @EventId
                    ORDER BY r.RegistrationDate DESC", new { EventId = eventId });
            }
        }

        public IEnumerable<Registration> GetRegistrationsByUserId(string userId)
        {
            using (var connection = DatabaseConfig.GetConnection())
            {
                connection.Open();
                return connection.Query<Registration, Event, Registration>(@"
                    SELECT r.*, e.*
                    FROM Registrations r
                    JOIN Events e ON r.EventId = e.EventId
                    WHERE r.UserId = @UserId
                    ORDER BY e.EventDate DESC",
                    (registration, eventEntity) =>
                    {
                        registration.Event = eventEntity;
                        return registration;
                    },
                    new { UserId = userId },
                    splitOn: "EventId");
            }
        }

        public Registration GetRegistration(int eventId, string userId)
        {
            using (var connection = DatabaseConfig.GetConnection())
            {
                connection.Open();
                return connection.QueryFirstOrDefault<Registration>(@"
                    SELECT * FROM Registrations
                    WHERE EventId = @EventId AND UserId = @UserId",
                    new { EventId = eventId, UserId = userId });
            }
        }

        public int CreateRegistration(Registration registration)
        {
            using (var connection = DatabaseConfig.GetConnection())
            {
                connection.Open();
                var sql = @"
                    INSERT INTO Registrations (EventId, UserId, RegistrationDate)
                    VALUES (@EventId, @UserId, @RegistrationDate);
                    SELECT last_insert_rowid();";

                return connection.ExecuteScalar<int>(sql, new
                {
                    registration.EventId,
                    registration.UserId,
                    RegistrationDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                });
            }
        }

        public bool CancelRegistration(int eventId, string userId)
        {
            using (var connection = DatabaseConfig.GetConnection())
            {
                connection.Open();
                int rowsAffected = connection.Execute(@"
                    DELETE FROM Registrations
                    WHERE EventId = @EventId AND UserId = @UserId",
                    new { EventId = eventId, UserId = userId });

                return rowsAffected > 0;
            }
        }
    }
}