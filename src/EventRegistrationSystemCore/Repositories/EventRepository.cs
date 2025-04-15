using Dapper;
using EventRegistrationSystemCore.Models;
using System.Data;

namespace EventRegistrationSystemCore.Repositories
{
    public class EventRepository : IEventRepository
    {
        private readonly IDbConnection _connection;

        public EventRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public IEnumerable<Event> GetFeaturedEvents(int count)
        {
            _connection.Open();
            try
            {
                return _connection.Query<Event>(@"
                    SELECT e.*, 
                    (SELECT COUNT(*) FROM Registrations r WHERE r.EventId = e.EventId) AS RegistrationCount
                    FROM Events e
                    ORDER BY e.EventDate
                    LIMIT @Count", new { Count = count });
            }
            finally
            {
                _connection.Close();
            }
        }

        public IEnumerable<Event> GetUpcomingEvents(int count)
        {
            _connection.Open();
            try
            {
                return _connection.Query<Event>(@"
                    SELECT e.*, 
                    (SELECT COUNT(*) FROM Registrations r WHERE r.EventId = e.EventId) AS RegistrationCount
                    FROM Events e
                    WHERE e.EventDate > CURRENT_TIMESTAMP
                    ORDER BY e.EventDate
                    LIMIT @Count", new { Count = count });
            }
            finally
            {
                _connection.Close();
            }
        }
    }
}
