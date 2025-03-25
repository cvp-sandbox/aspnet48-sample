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
                
            var registeredUsers = await _context.Registrations
                .CountAsync(r => r.Event != null && r.Event.EventDate > now);
                
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
            
            return await _context.Registrations
                .Include(r => r.Event)
                .Where(r => r.UserName == userName && 
                       r.Event != null && 
                       r.Event.EventDate > now && 
                       r.Event.EventDate <= nextWeek)
                .OrderBy(r => r.Event.EventDate)
                .ToListAsync();
        }
    }
}
