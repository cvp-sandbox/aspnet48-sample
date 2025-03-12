# Event Management System - Technical Specifications

## Overview
This document outlines the technical specifications for developing an Event Management System using ASP.NET WebForms with .NET Framework 4.8, Bootstrap, jQuery, and SQLite database.

## Technology Stack
- **Backend Framework**: ASP.NET WebForms (.NET Framework 4.8)
- **Database**: SQLite
- **Frontend**: Bootstrap 4.6, jQuery 3.6.0
- **Authentication**: ASP.NET Identity (customized for SQLite)
- **Export Formats**: iCal/ICS for calendar events, CSV for data export

## System Architecture

### Database Architecture

#### Database Schema
Below is the database schema for the Event Management System:

```sql
-- Users and Authentication Tables
CREATE TABLE Users (
    UserId INTEGER PRIMARY KEY AUTOINCREMENT,
    Username VARCHAR(50) NOT NULL UNIQUE,
    Email VARCHAR(100) NOT NULL UNIQUE,
    PasswordHash VARCHAR(128) NOT NULL,
    Salt VARCHAR(128) NOT NULL,
    FirstName VARCHAR(50),
    LastName VARCHAR(50),
    PhoneNumber VARCHAR(20),
    CreatedDate DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    LastLoginDate DATETIME,
    IsActive BOOLEAN NOT NULL DEFAULT 1
);

CREATE TABLE Roles (
    RoleId INTEGER PRIMARY KEY AUTOINCREMENT,
    RoleName VARCHAR(50) NOT NULL UNIQUE
);

CREATE TABLE UserRoles (
    UserRoleId INTEGER PRIMARY KEY AUTOINCREMENT,
    UserId INTEGER NOT NULL,
    RoleId INTEGER NOT NULL,
    FOREIGN KEY (UserId) REFERENCES Users(UserId),
    FOREIGN KEY (RoleId) REFERENCES Roles(RoleId),
    UNIQUE(UserId, RoleId)
);

-- Core Event Management Tables
CREATE TABLE Events (
    EventId INTEGER PRIMARY KEY AUTOINCREMENT,
    Title VARCHAR(100) NOT NULL,
    Description TEXT,
    StartDate DATETIME NOT NULL,
    EndDate DATETIME NOT NULL,
    RegistrationDeadline DATETIME NOT NULL,
    VenueId INTEGER,
    OrganizerId INTEGER NOT NULL,
    MaxAttendees INTEGER,
    IsPublished BOOLEAN NOT NULL DEFAULT 0,
    CreatedDate DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    LastUpdatedDate DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (VenueId) REFERENCES Venues(VenueId),
    FOREIGN KEY (OrganizerId) REFERENCES Users(UserId)
);

CREATE TABLE Venues (
    VenueId INTEGER PRIMARY KEY AUTOINCREMENT,
    Name VARCHAR(100) NOT NULL,
    Address VARCHAR(200) NOT NULL,
    City VARCHAR(50) NOT NULL,
    State VARCHAR(50),
    ZipCode VARCHAR(20),
    Country VARCHAR(50) NOT NULL,
    Capacity INTEGER,
    ContactInfo VARCHAR(200)
);

CREATE TABLE Rooms (
    RoomId INTEGER PRIMARY KEY AUTOINCREMENT,
    VenueId INTEGER NOT NULL,
    Name VARCHAR(50) NOT NULL,
    Capacity INTEGER,
    FOREIGN KEY (VenueId) REFERENCES Venues(VenueId)
);

CREATE TABLE Sessions (
    SessionId INTEGER PRIMARY KEY AUTOINCREMENT,
    EventId INTEGER NOT NULL,
    Title VARCHAR(100) NOT NULL,
    Description TEXT,
    StartTime DATETIME NOT NULL,
    EndTime DATETIME NOT NULL,
    RoomId INTEGER,
    MaxAttendees INTEGER,
    FOREIGN KEY (EventId) REFERENCES Events(EventId),
    FOREIGN KEY (RoomId) REFERENCES Rooms(RoomId)
);

CREATE TABLE Speakers (
    SpeakerId INTEGER PRIMARY KEY AUTOINCREMENT,
    UserId INTEGER NOT NULL,
    Bio TEXT,
    FOREIGN KEY (UserId) REFERENCES Users(UserId)
);

CREATE TABLE SessionSpeakers (
    SessionSpeakerId INTEGER PRIMARY KEY AUTOINCREMENT,
    SessionId INTEGER NOT NULL,
    SpeakerId INTEGER NOT NULL,
    FOREIGN KEY (SessionId) REFERENCES Sessions(SessionId),
    FOREIGN KEY (SpeakerId) REFERENCES Speakers(SpeakerId),
    UNIQUE(SessionId, SpeakerId)
);

CREATE TABLE TicketTypes (
    TicketTypeId INTEGER PRIMARY KEY AUTOINCREMENT,
    EventId INTEGER NOT NULL,
    Name VARCHAR(50) NOT NULL,
    Description TEXT,
    Price DECIMAL(10,2) NOT NULL,
    AvailableQuantity INTEGER,
    SaleStartDate DATETIME NOT NULL,
    SaleEndDate DATETIME NOT NULL,
    FOREIGN KEY (EventId) REFERENCES Events(EventId)
);

CREATE TABLE Registrations (
    RegistrationId INTEGER PRIMARY KEY AUTOINCREMENT,
    EventId INTEGER NOT NULL,
    UserId INTEGER NOT NULL,
    TicketTypeId INTEGER NOT NULL,
    RegistrationDate DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    Status VARCHAR(20) NOT NULL, -- "Confirmed", "Cancelled", "Pending"
    FOREIGN KEY (EventId) REFERENCES Events(EventId),
    FOREIGN KEY (UserId) REFERENCES Users(UserId),
    FOREIGN KEY (TicketTypeId) REFERENCES TicketTypes(TicketTypeId)
);

CREATE TABLE SessionAttendees (
    SessionAttendeeId INTEGER PRIMARY KEY AUTOINCREMENT,
    SessionId INTEGER NOT NULL,
    RegistrationId INTEGER NOT NULL,
    FOREIGN KEY (SessionId) REFERENCES Sessions(SessionId),
    FOREIGN KEY (RegistrationId) REFERENCES Registrations(RegistrationId),
    UNIQUE(SessionId, RegistrationId)
);
```

### Sample Data Script

```sql
-- Insert Roles
INSERT INTO Roles (RoleName) VALUES ('Admin');
INSERT INTO Roles (RoleName) VALUES ('EventOrganizer');
INSERT INTO Roles (RoleName) VALUES ('Attendee');
INSERT INTO Roles (RoleName) VALUES ('Speaker');

-- Insert Sample Users (Password: 'Password123!' for all users)
INSERT INTO Users (Username, Email, PasswordHash, Salt, FirstName, LastName, PhoneNumber) 
VALUES ('admin', 'admin@example.com', '9ED8D3FCAB8B4CD323E158C91AMCE1EB1B9127ED33D4FA5A3192D139952', 'XYZ123', 'Admin', 'User', '555-123-4567');

INSERT INTO Users (Username, Email, PasswordHash, Salt, FirstName, LastName, PhoneNumber) 
VALUES ('organizer1', 'organizer1@example.com', '9ED8D3FCAB8B4CD323E158C91AMCE1EB1B9127ED33D4FA5A3192D139952', 'XYZ123', 'John', 'Doe', '555-234-5678');

INSERT INTO Users (Username, Email, PasswordHash, Salt, FirstName, LastName, PhoneNumber) 
VALUES ('organizer2', 'organizer2@example.com', '9ED8D3FCAB8B4CD323E158C91AMCE1EB1B9127ED33D4FA5A3192D139952', 'XYZ123', 'Jane', 'Smith', '555-345-6789');

INSERT INTO Users (Username, Email, PasswordHash, Salt, FirstName, LastName, PhoneNumber) 
VALUES ('attendee1', 'attendee1@example.com', '9ED8D3FCAB8B4CD323E158C91AMCE1EB1B9127ED33D4FA5A3192D139952', 'XYZ123', 'Robert', 'Johnson', '555-456-7890');

INSERT INTO Users (Username, Email, PasswordHash, Salt, FirstName, LastName, PhoneNumber) 
VALUES ('attendee2', 'attendee2@example.com', '9ED8D3FCAB8B4CD323E158C91AMCE1EB1B9127ED33D4FA5A3192D139952', 'XYZ123', 'Sarah', 'Williams', '555-567-8901');

INSERT INTO Users (Username, Email, PasswordHash, Salt, FirstName, LastName, PhoneNumber) 
VALUES ('speaker1', 'speaker1@example.com', '9ED8D3FCAB8B4CD323E158C91AMCE1EB1B9127ED33D4FA5A3192D139952', 'XYZ123', 'Michael', 'Brown', '555-678-9012');

INSERT INTO Users (Username, Email, PasswordHash, Salt, FirstName, LastName, PhoneNumber) 
VALUES ('speaker2', 'speaker2@example.com', '9ED8D3FCAB8B4CD323E158C91AMCE1EB1B9127ED33D4FA5A3192D139952', 'XYZ123', 'Emily', 'Davis', '555-789-0123');

-- Assign Roles to Users
INSERT INTO UserRoles (UserId, RoleId) VALUES (1, 1); -- Admin role to admin user
INSERT INTO UserRoles (UserId, RoleId) VALUES (2, 2); -- EventOrganizer role to organizer1
INSERT INTO UserRoles (UserId, RoleId) VALUES (3, 2); -- EventOrganizer role to organizer2
INSERT INTO UserRoles (UserId, RoleId) VALUES (4, 3); -- Attendee role to attendee1
INSERT INTO UserRoles (UserId, RoleId) VALUES (5, 3); -- Attendee role to attendee2
INSERT INTO UserRoles (UserId, RoleId) VALUES (6, 4); -- Speaker role to speaker1
INSERT INTO UserRoles (UserId, RoleId) VALUES (7, 4); -- Speaker role to speaker2
INSERT INTO UserRoles (UserId, RoleId) VALUES (6, 3); -- Attendee role to speaker1
INSERT INTO UserRoles (UserId, RoleId) VALUES (7, 3); -- Attendee role to speaker2

-- Insert Speakers
INSERT INTO Speakers (UserId, Bio) 
VALUES (6, 'Michael Brown is an experienced technology consultant with over 15 years in the industry.');

INSERT INTO Speakers (UserId, Bio) 
VALUES (7, 'Emily Davis is a renowned expert in digital marketing and social media strategies.');

-- Insert Venues
INSERT INTO Venues (Name, Address, City, State, ZipCode, Country, Capacity, ContactInfo)
VALUES ('Tech Conference Center', '123 Main Street', 'San Francisco', 'CA', '94105', 'USA', 1000, 'contact@techconferencecenter.com');

INSERT INTO Venues (Name, Address, City, State, ZipCode, Country, Capacity, ContactInfo)
VALUES ('Business Innovation Hub', '456 Market Street', 'New York', 'NY', '10001', 'USA', 750, 'info@businesshub.com');

-- Insert Rooms
INSERT INTO Rooms (VenueId, Name, Capacity)
VALUES (1, 'Main Hall A', 500);

INSERT INTO Rooms (VenueId, Name, Capacity)
VALUES (1, 'Workshop Room B', 100);

INSERT INTO Rooms (VenueId, Name, Capacity)
VALUES (1, 'Conference Room C', 200);

INSERT INTO Rooms (VenueId, Name, Capacity)
VALUES (2, 'Auditorium', 400);

INSERT INTO Rooms (VenueId, Name, Capacity)
VALUES (2, 'Seminar Room 1', 150);

INSERT INTO Rooms (VenueId, Name, Capacity)
VALUES (2, 'Seminar Room 2', 150);

-- Insert Events
INSERT INTO Events (Title, Description, StartDate, EndDate, RegistrationDeadline, VenueId, OrganizerId, MaxAttendees, IsPublished)
VALUES ('Annual Tech Summit 2023', 'Join us for the latest in technology trends and innovations.', '2023-10-15 09:00:00', '2023-10-17 17:00:00', '2023-10-01 23:59:59', 1, 2, 800, 1);

INSERT INTO Events (Title, Description, StartDate, EndDate, RegistrationDeadline, VenueId, OrganizerId, MaxAttendees, IsPublished)
VALUES ('Digital Marketing Workshop', 'Learn the latest digital marketing strategies from industry experts.', '2023-11-05 10:00:00', '2023-11-05 16:00:00', '2023-10-25 23:59:59', 2, 3, 200, 1);

INSERT INTO Events (Title, Description, StartDate, EndDate, RegistrationDeadline, VenueId, OrganizerId, MaxAttendees, IsPublished)
VALUES ('Future of AI Conference', 'Explore the future of artificial intelligence and its impact on business.', '2023-12-10 09:00:00', '2023-12-12 17:00:00', '2023-11-30 23:59:59', 1, 2, 600, 0);

-- Insert Sessions
INSERT INTO Sessions (EventId, Title, Description, StartTime, EndTime, RoomId, MaxAttendees)
VALUES (1, 'Keynote: Future of Technology', 'Opening keynote discussing the future of technology.', '2023-10-15 09:30:00', '2023-10-15 10:30:00', 1, 500);

INSERT INTO Sessions (EventId, Title, Description, StartTime, EndTime, RoomId, MaxAttendees)
VALUES (1, 'Cloud Computing Workshop', 'Hands-on workshop on cloud computing solutions.', '2023-10-15 11:00:00', '2023-10-15 13:00:00', 2, 100);

INSERT INTO Sessions (EventId, Title, Description, StartTime, EndTime, RoomId, MaxAttendees)
VALUES (1, 'Cybersecurity Panel', 'Expert panel discussing the latest in cybersecurity.', '2023-10-16 09:30:00', '2023-10-16 11:00:00', 3, 200);

INSERT INTO Sessions (EventId, Title, Description, StartTime, EndTime, RoomId, MaxAttendees)
VALUES (2, 'Social Media Strategy Masterclass', 'Comprehensive masterclass on social media strategy.', '2023-11-05 10:30:00', '2023-11-05 12:30:00', 4, 150);

INSERT INTO Sessions (EventId, Title, Description, StartTime, EndTime, RoomId, MaxAttendees)
VALUES (2, 'SEO Best Practices Workshop', 'Learn the latest SEO techniques and best practices.', '2023-11-05 13:30:00', '2023-11-05 15:30:00', 5, 150);

-- Assign Speakers to Sessions
INSERT INTO SessionSpeakers (SessionId, SpeakerId)
VALUES (1, 1);

INSERT INTO SessionSpeakers (SessionId, SpeakerId)
VALUES (2, 1);

INSERT INTO SessionSpeakers (SessionId, SpeakerId)
VALUES (3, 1);

INSERT INTO SessionSpeakers (SessionId, SpeakerId)
VALUES (4, 2);

INSERT INTO SessionSpeakers (SessionId, SpeakerId)
VALUES (5, 2);

-- Insert Ticket Types
INSERT INTO TicketTypes (EventId, Name, Description, Price, AvailableQuantity, SaleStartDate, SaleEndDate)
VALUES (1, 'Early Bird', 'Early bird discount ticket.', 199.99, 300, '2023-08-01 00:00:00', '2023-09-01 23:59:59');

INSERT INTO TicketTypes (EventId, Name, Description, Price, AvailableQuantity, SaleStartDate, SaleEndDate)
VALUES (1, 'Regular', 'Regular admission ticket.', 299.99, 400, '2023-09-02 00:00:00', '2023-10-01 23:59:59');

INSERT INTO TicketTypes (EventId, Name, Description, Price, AvailableQuantity, SaleStartDate, SaleEndDate)
VALUES (1, 'VIP', 'VIP ticket with exclusive access.', 499.99, 100, '2023-08-01 00:00:00', '2023-10-01 23:59:59');

INSERT INTO TicketTypes (EventId, Name, Description, Price, AvailableQuantity, SaleStartDate, SaleEndDate)
VALUES (2, 'Workshop Pass', 'Full-day workshop admission.', 149.99, 200, '2023-09-15 00:00:00', '2023-10-25 23:59:59');

-- Insert Registrations
INSERT INTO Registrations (EventId, UserId, TicketTypeId, RegistrationDate, Status)
VALUES (1, 4, 1, '2023-08-15 14:30:00', 'Confirmed');

INSERT INTO Registrations (EventId, UserId, TicketTypeId, RegistrationDate, Status)
VALUES (1, 5, 3, '2023-08-20 10:15:00', 'Confirmed');

INSERT INTO Registrations (EventId, UserId, TicketTypeId, RegistrationDate, Status)
VALUES (2, 4, 4, '2023-09-25 16:45:00', 'Confirmed');

-- Insert Session Attendees
INSERT INTO SessionAttendees (SessionId, RegistrationId)
VALUES (1, 1);

INSERT INTO SessionAttendees (SessionId, RegistrationId)
VALUES (2, 1);

INSERT INTO SessionAttendees (SessionId, RegistrationId)
VALUES (1, 2);

INSERT INTO SessionAttendees (SessionId, RegistrationId)
VALUES (3, 2);

INSERT INTO SessionAttendees (SessionId, RegistrationId)
VALUES (4, 3);

INSERT INTO SessionAttendees (SessionId, RegistrationId)
VALUES (5, 3);
```

## Application Structure

### Project Setup
1. Create a new ASP.NET Web Application (.NET Framework 4.8) project
2. Select the WebForms template
3. Configure for Individual User Accounts authentication

### Key Components

#### 1. Data Access Layer (DAL)
Create a folder named "DAL" in the project and implement the following components:

```csharp
// EventManagerContext.cs
using System;
using System.Data.Entity;
using System.Data.SQLite;
using System.Data.SQLite.EF6;
using System.IO;

namespace EventManagement.DAL
{
    public class EventManagerContext : DbContext
    {
        public EventManagerContext() : base(new SQLiteConnection()
        {
            ConnectionString = new SQLiteConnectionStringBuilder()
            {
                DataSource = Path.Combine(AppDomain.CurrentDomain.GetData("DataDirectory").ToString(), "EventManager.db"),
                ForeignKeys = true
            }.ConnectionString
        }, true)
        {
            // SQLite initialization
            Database.SetInitializer<EventManagerContext>(null);
        }

        // DbSets for all entities
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Venue> Venues { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<Speaker> Speakers { get; set; }
        public DbSet<SessionSpeaker> SessionSpeakers { get; set; }
        public DbSet<TicketType> TicketTypes { get; set; }
        public DbSet<Registration> Registrations { get; set; }
        public DbSet<SessionAttendee> SessionAttendees { get; set; }
        
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Configure entity relationships and constraints
            // Implement entity configuration for each model
            
            base.OnModelCreating(modelBuilder);
        }
    }
}
```

#### 2. Model Classes
Create a folder named "Models" in the project with the following model classes:

```csharp
// User.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventManagement.Models
{
    [Table("Users")]
    public class User
    {
        [Key]
        public int UserId { get; set; }
        
        [Required, StringLength(50)]
        public string Username { get; set; }
        
        [Required, StringLength(100), EmailAddress]
        public string Email { get; set; }
        
        [Required, StringLength(128)]
        public string PasswordHash { get; set; }
        
        [Required, StringLength(128)]
        public string Salt { get; set; }
        
        [StringLength(50)]
        public string FirstName { get; set; }
        
        [StringLength(50)]
        public string LastName { get; set; }
        
        [StringLength(20)]
        public string PhoneNumber { get; set; }
        
        [Required]
        public DateTime CreatedDate { get; set; }
        
        public DateTime? LastLoginDate { get; set; }
        
        [Required]
        public bool IsActive { get; set; }
        
        // Navigation properties
        public virtual ICollection<UserRole> UserRoles { get; set; }
        public virtual ICollection<Event> OrganizedEvents { get; set; }
        public virtual Speaker Speaker { get; set; }
        public virtual ICollection<Registration> Registrations { get; set; }
    }
}
```

Include similar model classes for all database entities, ensuring they match the database schema.

#### 3. Repository Pattern Implementation
Create a folder named "Repositories" to implement the repository pattern:

```csharp
// IRepository.cs
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace EventManagement.Repositories
{
    public interface IRepository<TEntity> where TEntity : class
    {
        TEntity Get(int id);
        IEnumerable<TEntity> GetAll();
        IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate);
        
        void Add(TEntity entity);
        void AddRange(IEnumerable<TEntity> entities);
        
        void Update(TEntity entity);
        
        void Remove(TEntity entity);
        void RemoveRange(IEnumerable<TEntity> entities);
    }
}
```

```csharp
// Repository.cs
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using EventManagement.DAL;

namespace EventManagement.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected readonly EventManagerContext Context;
        protected readonly DbSet<TEntity> DbSet;

        public Repository(EventManagerContext context)
        {
            Context = context;
            DbSet = context.Set<TEntity>();
        }

        public TEntity Get(int id)
        {
            return DbSet.Find(id);
        }

        public IEnumerable<TEntity> GetAll()
        {
            return DbSet.ToList();
        }

        public IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
        {
            return DbSet.Where(predicate).ToList();
        }

        public void Add(TEntity entity)
        {
            DbSet.Add(entity);
        }

        public void AddRange(IEnumerable<TEntity> entities)
        {
            DbSet.AddRange(entities);
        }

        public void Update(TEntity entity)
        {
            DbSet.Attach(entity);
            Context.Entry(entity).State = EntityState.Modified;
        }

        public void Remove(TEntity entity)
        {
            DbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<TEntity> entities)
        {
            DbSet.RemoveRange(entities);
        }
    }
}
```

Create specific repository classes for each entity type (UserRepository, EventRepository, etc.).

#### 4. Service Layer
Create a folder named "Services" to implement the business logic:

```csharp
// IEventService.cs
using System;
using System.Collections.Generic;
using EventManagement.Models;

namespace EventManagement.Services
{
    public interface IEventService
    {
        IEnumerable<Event> GetAllEvents();
        IEnumerable<Event> GetPublishedEvents();
        IEnumerable<Event> GetEventsOrganizedByUser(int userId);
        Event GetEventById(int eventId);
        void CreateEvent(Event @event);
        void UpdateEvent(Event @event);
        void DeleteEvent(int eventId);
        bool IsEventFull(int eventId);
    }
}
```

```csharp
// EventService.cs
using System;
using System.Collections.Generic;
using System.Linq;
using EventManagement.Models;
using EventManagement.Repositories;

namespace EventManagement.Services
{
    public class EventService : IEventService
    {
        private readonly IRepository<Event> _eventRepository;
        private readonly IRepository<Registration> _registrationRepository;
        private readonly IUnitOfWork _unitOfWork;

        public EventService(IRepository<Event> eventRepository, 
                           IRepository<Registration> registrationRepository,
                           IUnitOfWork unitOfWork)
        {
            _eventRepository = eventRepository;
            _registrationRepository = registrationRepository;
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<Event> GetAllEvents()
        {
            return _eventRepository.GetAll();
        }

        public IEnumerable<Event> GetPublishedEvents()
        {
            return _eventRepository.Find(e => e.IsPublished);
        }

        public IEnumerable<Event> GetEventsOrganizedByUser(int userId)
        {
            return _eventRepository.Find(e => e.OrganizerId == userId);
        }

        public Event GetEventById(int eventId)
        {
            return _eventRepository.Get(eventId);
        }

        public void CreateEvent(Event @event)
        {
            _eventRepository.Add(@event);
            _unitOfWork.Complete();
        }

        public void UpdateEvent(Event @event)
        {
            _eventRepository.Update(@event);
            _unitOfWork.Complete();
        }

        public void DeleteEvent(int eventId)
        {
            var @event = _eventRepository.Get(eventId);
            if (@event != null)
            {
                _eventRepository.Remove(@event);
                _unitOfWork.Complete();
            }
        }

        public bool IsEventFull(int eventId)
        {
            var @event = _eventRepository.Get(eventId);
            if (@event == null || !@event.MaxAttendees.HasValue)
                return false;

            var registrationCount = _registrationRepository
                .Find(r => r.EventId == eventId && r.Status == "Confirmed")
                .Count();

            return registrationCount >= @event.MaxAttendees.Value;
        }
    }
}
```

Create similar service interfaces and implementations for all entities.

#### 5. Authentication System
Implement a custom ASP.NET Identity system with SQLite support:

```csharp
// CustomMembershipProvider.cs
using System;
using System.Web.Security;
using EventManagement.Services;

namespace EventManagement.Authentication
{
    public class CustomMembershipProvider : MembershipProvider
    {
        private readonly IUserService _userService;

        public CustomMembershipProvider(IUserService userService)
        {
            _userService = userService;
        }

        public override bool ValidateUser(string username, string password)
        {
            return _userService.ValidateUser(username, password);
        }

        // Implement other required methods from MembershipProvider
        // ...
    }
}
```

```csharp
// CustomRoleProvider.cs
using System;
using System.Web.Security;
using EventManagement.Services;

namespace EventManagement.Authentication
{
    public class CustomRoleProvider : RoleProvider
    {
        private readonly IRoleService _roleService;

        public CustomRoleProvider(IRoleService roleService)
        {
            _roleService = roleService;
        }

        public override string[] GetRolesForUser(string username)
        {
            return _roleService.GetRolesForUser(username);
        }

        // Implement other required methods from RoleProvider
        // ...
    }
}
```

#### 6. Data Export Utilities
Create utilities for exporting data to CSV and iCal formats:

```csharp
// CsvExporter.cs
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace EventManagement.Utilities
{
    public class CsvExporter
    {
        public static string ExportToCsv<T>(IEnumerable<T> data, string[] headers = null)
        {
            StringBuilder sb = new StringBuilder();
            
            // Add headers
            if (headers != null && headers.Length > 0)
            {
                sb.AppendLine(string.Join(",", headers));
            }
            else
            {
                var properties = typeof(T).GetProperties();
                sb.AppendLine(string.Join(",", properties.Select(p => p.Name)));
            }
            
            // Add data rows
            foreach (var item in data)
            {
                var properties = typeof(T).GetProperties();
                sb.AppendLine(string.Join(",", properties.Select(p => 
                {
                    var value = p.GetValue(item);
                    return value != null ? value.ToString() : string.Empty;
                })));
            }
            
            return sb.ToString();
        }
    }
}
```

```csharp
// ICalExporter.cs
using System;
using System.Text;
using EventManagement.Models;

namespace EventManagement.Utilities
{
    public class ICalExporter
    {
        public static string ExportToICal(Event @event)
        {
            StringBuilder sb = new StringBuilder();
            
            sb.AppendLine("BEGIN:VCALENDAR");
            sb.AppendLine("VERSION:2.0");
            sb.AppendLine("PRODID:-//EventManagement//EN");
            
            sb.AppendLine("BEGIN:VEVENT");
            sb.AppendLine($"UID:{@event.EventId}@eventmanagement.com");
            sb.AppendLine($"DTSTAMP:{DateTime.UtcNow:yyyyMMddTHHmmssZ}");
            sb.AppendLine($"DTSTART:{@event.StartDate:yyyyMMddTHHmmssZ}");
            sb.AppendLine($"DTEND:{@event.EndDate:yyyyMMddTHHmmssZ}");
            sb.AppendLine($"SUMMARY:{@event.Title}");
            sb.AppendLine($"DESCRIPTION:{@event.Description}");
            // Add location information if venue exists
            if (@event.Venue != null)
            {
                sb.AppendLine($"LOCATION:{@event.Venue.Name}, {@event.Venue.Address}, {@event.Venue.City}, {@event.Venue.State}, {@event.Venue.Country}");
            }
            sb.AppendLine("END:VEVENT");
            
            sb.AppendLine("END:VCALENDAR");
            
            return sb.ToString();
        }
        
        public static string ExportSessionsToICal(IEnumerable<Session> sessions, Event @event)
        {
            StringBuilder sb = new StringBuilder();
            
            sb.AppendLine("BEGIN:VCALENDAR");
            sb