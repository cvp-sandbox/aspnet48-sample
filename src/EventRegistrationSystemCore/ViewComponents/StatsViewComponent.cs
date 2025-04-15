using Microsoft.AspNetCore.Mvc;
using System.Data;
using Dapper;

namespace EventRegistrationSystemCore.ViewComponents
{
    public class StatsViewComponent : ViewComponent
    {
        private readonly IDbConnection _connection;

        public StatsViewComponent(IDbConnection connection)
        {
            _connection = connection;
        }

        public IViewComponentResult Invoke()
        {
            _connection.Open();
            try
            {
                var stats = _connection.Query(@"
                    select 'ActiveEvents' as StatLabel, count(*) AS StatValue
                    from Events
                    where EventDate > CURRENT_TIMESTAMP

                    UNION

                    select 'ThisWeeksEvents' as StatLabel, count(*) as StatValue
                    from Events
                    WHERE EventDate BETWEEN CURRENT_TIMESTAMP AND DATE('now', '+7 days')

                    UNION

                    select 'RegisteredUsers' as StatLabel, Count(RegistrationId) as StatValue
                    from Registrations r 
                    join Events e on r.EventId = e.EventId
                    where EventDate > CURRENT_TIMESTAMP");
                
                return View(stats);
            }
            finally
            {
                _connection.Close();
            }
        }
    }
}
