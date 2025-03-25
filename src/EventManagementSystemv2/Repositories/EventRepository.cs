using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EventManagementSystemv2.Data;
using EventManagementSystemv2.Models;
using EventManagementSystemv2.Repositories.Interfaces;

namespace EventManagementSystemv2.Repositories
{
    public class EventRepository : IEventRepository
    {
        private readonly ApplicationDbContext _context;

        public EventRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Event>> GetAllEventsAsync()
        {
            return await _context.Events
                .OrderByDescending(e => e.EventDate)
                .ToListAsync();
        }

        public async Task<Event?> GetEventByIdAsync(int eventId)
        {
            return await _context.Events.FindAsync(eventId);
        }

        public async Task<IEnumerable<Event>> GetEventsByCreatorAsync(string userId)
        {
            return await _context.Events
                .Where(e => e.CreatedBy == userId)
                .OrderByDescending(e => e.EventDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Event>> GetFeaturedEventsAsync(int count = 3)
        {
            // This replicates the SQL query in the original HomeController
            var events = await _context.Events
                .OrderBy(e => e.EventDate)
                .Take(count)
                .ToListAsync();

            // Load registration counts
            foreach (var eventItem in events)
            {
                eventItem.RegistrationCount = await _context.Registrations
                    .CountAsync(r => r.EventId == eventItem.EventId);
            }

            return events;
        }

        public async Task<int> CreateEventAsync(Event eventEntity)
        {
            eventEntity.CreatedDate = DateTime.Now;
            _context.Events.Add(eventEntity);
            await _context.SaveChangesAsync();
            return eventEntity.EventId;
        }

        public async Task<bool> UpdateEventAsync(Event eventEntity)
        {
            _context.Events.Update(eventEntity);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> DeleteEventAsync(int eventId)
        {
            // First delete registrations
            var registrations = await _context.Registrations
                .Where(r => r.EventId == eventId)
                .ToListAsync();
            
            _context.Registrations.RemoveRange(registrations);
            
            // Then delete the event
            var eventEntity = await _context.Events.FindAsync(eventId);
            if (eventEntity == null)
                return false;
                
            _context.Events.Remove(eventEntity);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<int> GetRegistrationCountAsync(int eventId)
        {
            return await _context.Registrations
                .CountAsync(r => r.EventId == eventId);
        }

        public async Task<IEnumerable<dynamic>> GetStatsAsync()
        {
            // This replicates the SQL query in the original HomeController
            var now = DateTime.Now;
            var nextWeek = now.AddDays(7);
            
            var activeEvents = await _context.Events
                .CountAsync(e => e.EventDate > now);
                
            var thisWeeksEvents = await _context.Events
                .CountAsync(e => e.EventDate > now && e.EventDate <= nextWeek);
                
            // Count registrations for future events by joining Events and Registrations
            var registeredUsers = await _context.Registrations
                .Join(_context.Events,
                    r => r.EventId,
                    e => e.EventId,
                    (r, e) => new { Registration = r, Event = e })
                .CountAsync(joined => joined.Event.EventDate > now);
                
            var stats = new List<dynamic>
            {
                new { StatLabel = "ActiveEvents", StatValue = activeEvents },
                new { StatLabel = "ThisWeeksEvents", StatValue = thisWeeksEvents },
                new { StatLabel = "RegisteredUsers", StatValue = registeredUsers }
            };
            
            return stats;
        }

        public async Task<IEnumerable<Registration>> GetUpcomingUserEventsAsync(string userName)
        {
            var now = DateTime.Now;
            var nextWeek = now.AddDays(7);
            
            // Join Registrations with Events to filter by event date
            var query = from r in _context.Registrations
                        join e in _context.Events on r.EventId equals e.EventId
                        where r.UserName == userName && 
                              e.EventDate > now && 
                              e.EventDate <= nextWeek
                        orderby e.EventDate
                        select r;
            
            // Load the events for each registration
            var registrations = await query.ToListAsync();
            
            // Manually load the Event navigation property for each registration
            foreach (var registration in registrations)
            {
                registration.Event = await _context.Events.FindAsync(registration.EventId);
            }
            
            return registrations;
        }
    }
}
