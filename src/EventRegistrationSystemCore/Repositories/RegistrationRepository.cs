using Dapper;
using EventRegistrationSystemCore.Models;
using System.Data;

namespace EventRegistrationSystemCore.Repositories
{
    public class RegistrationRepository : IRegistrationRepository
    {
        private readonly IDbConnection _connection;

        public RegistrationRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public IEnumerable<Registration> GetUserUpcomingEvents(string userId)
        {
            _connection.Open();
            try
            {
                return _connection.Query<Registration, Event, Registration>(@"
                    SELECT r.*, e.*
                    FROM Registrations r
                    JOIN Events e ON r.EventId = e.EventId
                    JOIN AspNetUsers u on u.Id = r.UserId
                    WHERE u.Id = @UserId AND e.EventDate BETWEEN CURRENT_TIMESTAMP AND DATE('now', '+7 days')
                    ORDER BY e.EventDate DESC",
                    (registration, eventItem) => 
                    {
                        registration.Event = eventItem;
                        return registration;
                    },
                    new { UserId = userId },
                    splitOn: "EventId");
            }
            finally
            {
                _connection.Close();
            }
        }
    }
}
