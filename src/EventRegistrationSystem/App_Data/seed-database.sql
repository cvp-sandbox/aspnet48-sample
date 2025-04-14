-- SQLite
-- Seed data for Roles table
-- Note: The basic roles were already created in the CreateRolesTableSql in DatabaseConfig.cs
-- This adds additional roles if needed
-- INSERT OR IGNORE INTO Roles (Name) VALUES ('Admin');
-- INSERT OR IGNORE INTO Roles (Name) VALUES ('Organizer');
-- INSERT OR IGNORE INTO Roles (Name) VALUES ('User');

-- Seed sample users
-- Note: These are example passwords, in a real app passwords would be hashed
-- Password for all users below would be "P@ssw0rd1!" if properly hashed
INSERT INTO AspNetUsers (
    Id, 
    Email, 
    EmailConfirmed, 
    PasswordHash, 
    SecurityStamp, 
    PhoneNumber, 
    PhoneNumberConfirmed, 
    TwoFactorEnabled, 
    LockoutEndDateUtc, 
    LockoutEnabled, 
    AccessFailedCount, 
    UserName
) VALUES 
    ('7', 'admin@example.com', 1, 'AIa8HqoLO8bqYIvdA+sqAexxWufTrSvIJn1BW0aI+aZSHZ7kdGulu6N/x/b0UEL0Ag==', 'a7e8c7f0-c6d9-4b58-8a0e-b7d13592b87e', NULL, 0, 0, NULL, 1, 0, 'admin@example.com'),
    ('8', 'organizer1@example.com', 1, 'AIa8HqoLO8bqYIvdA+sqAexxWufTrSvIJn1BW0aI+aZSHZ7kdGulu6N/x/b0UEL0Ag==', 'b8f9d6e1-a2b3-4c5d-6e7f-8a9b0c1d2e3f', NULL, 0, 0, NULL, 1, 0, 'organizer1@example.com'),
    ('3', 'organizer2@example.com', 1, 'AIa8HqoLO8bqYIvdA+sqAexxWufTrSvIJn1BW0aI+aZSHZ7kdGulu6N/x/b0UEL0Ag==', 'c1d2e3f4-g5h6-i7j8-9k10-l11m12n13o14', NULL, 0, 0, NULL, 1, 0, 'organizer2@example.com'),
    ('4', 'user1@example.com', 1, 'AIa8HqoLO8bqYIvdA+sqAexxWufTrSvIJn1BW0aI+aZSHZ7kdGulu6N/x/b0UEL0Ag==', 'd4e5f6g7-h8i9-j10k-11l12-m13n14o15p', NULL, 0, 0, NULL, 1, 0, 'user1@example.com'),
    ('5', 'user2@example.com', 1, 'AIa8HqoLO8bqYIvdA+sqAexxWufTrSvIJn1BW0aI+aZSHZ7kdGulu6N/x/b0UEL0Ag==', 'e5f6g7h8-i9j10-k11l-12m13-n14o15p16', NULL, 0, 0, NULL, 1, 0, 'user2@example.com'),
    ('6', 'user3@example.com', 1, 'AIa8HqoLO8bqYIvdA+sqAexxWufTrSvIJn1BW0aI+aZSHZ7kdGulu6N/x/b0UEL0Ag==', 'f6g7h8i9-j10k11-l12m-13n14-o15p16q', NULL, 0, 0, NULL, 1, 0, 'user3@example.com');

-- Assign roles to users
INSERT INTO UserRoles (UserId, RoleId) VALUES ('7', (SELECT RoleId FROM Roles WHERE Name = 'Admin'));
INSERT INTO UserRoles (UserId, RoleId) VALUES ('8', (SELECT RoleId FROM Roles WHERE Name = 'Organizer'));
INSERT INTO UserRoles (UserId, RoleId) VALUES ('3', (SELECT RoleId FROM Roles WHERE Name = 'Organizer'));
INSERT INTO UserRoles (UserId, RoleId) VALUES ('4', (SELECT RoleId FROM Roles WHERE Name = 'User'));
INSERT INTO UserRoles (UserId, RoleId) VALUES ('5', (SELECT RoleId FROM Roles WHERE Name = 'User'));
INSERT INTO UserRoles (UserId, RoleId) VALUES ('6', (SELECT RoleId FROM Roles WHERE Name = 'User'));
-- Also make admin a user (multiple roles)
INSERT INTO UserRoles (UserId, RoleId) VALUES ('7', (SELECT RoleId FROM Roles WHERE Name = 'User'));

-- Create sample events
INSERT INTO Events (
    Name, 
    Description, 
    EventDate, 
    Location, 
    MaxAttendees, 
    CreatedBy, 
    CreatedDate
) VALUES 
    ('Introduction to ASP.NET MVC', 
     'Learn the basics of building web applications with ASP.NET MVC framework. This workshop covers controllers, views, models, and data access.', 
     '2025-03-15 10:00:00', 
     'Training Center, Room 305', 
     30, 
     '2', 
     '2025-01-10 09:00:00'),
     
    ('Annual Tech Summit', 
     'Join industry leaders for a day of insights, networking, and innovation discussions. Features keynote speakers and breakout sessions.', 
     '2025-04-05 09:00:00', 
     'Conference Center, Main Hall', 
     200, 
     '2', 
     '2025-01-15 14:30:00'),
     
    ('Developer Meetup', 
     'Casual gathering for developers to share ideas, collaborate, and enjoy refreshments. Open to all skill levels.', 
     '2025-03-22 18:00:00', 
     'Tech Hub Lounge', 
     50, 
     '3', 
     '2025-01-20 11:15:00'),
     
    ('Database Design Workshop', 
     'Hands-on workshop covering relational database design principles, normalization, and query optimization.', 
     '2025-03-30 13:00:00', 
     'Online - Zoom', 
     25, 
     '3', 
     '2025-01-22 16:45:00'),
     
    ('Mobile App Development Bootcamp', 
     'Intensive two-day course on building cross-platform mobile applications using modern frameworks.', 
     '2025-04-15 09:00:00', 
     'Tech Academy, Lab 4', 
     20, 
     '2', 
     '2025-01-25 10:30:00'),
     
    ('Web Security Fundamentals', 
     'Learn essential practices for securing web applications against common vulnerabilities and threats.', 
     '2025-05-10 10:00:00', 
     'Cybersecurity Center', 
     40, 
     '3', 
     '2025-02-01 13:00:00');

-- Add registrations for events
-- Event 1: 
INSERT INTO Registrations (EventId, UserId, RegistrationDate) VALUES (1, '1', '2025-01-12 08:15:00');
INSERT INTO Registrations (EventId, UserId, RegistrationDate) VALUES (1, '8', '2025-01-12 08:15:00');
INSERT INTO Registrations (EventId, UserId, RegistrationDate) VALUES (1, '4', '2025-01-13 12:30:00');
INSERT INTO Registrations (EventId, UserId, RegistrationDate) VALUES (1, '5', '2025-01-14 17:45:00');

-- Event 2: 
INSERT INTO Registrations (EventId, UserId, RegistrationDate) VALUES (2, '1', '2025-01-16 09:20:00');
INSERT INTO Registrations (EventId, UserId, RegistrationDate) VALUES (2, '3', '2025-01-17 14:10:00');
INSERT INTO Registrations (EventId, UserId, RegistrationDate) VALUES (2, '4', '2025-01-18 11:05:00');
INSERT INTO Registrations (EventId, UserId, RegistrationDate) VALUES (2, '5', '2025-01-19 16:30:00');
INSERT INTO Registrations (EventId, UserId, RegistrationDate) VALUES (2, '6', '2025-01-20 10:15:00');



-- Event 3: Developer Meetup
INSERT INTO Registrations (EventId, UserId, RegistrationDate) VALUES (3, '1', '2025-01-22 13:45:00');
INSERT INTO Registrations (EventId, UserId, RegistrationDate) VALUES (3, '2', '2025-01-23 09:30:00');
INSERT INTO Registrations (EventId, UserId, RegistrationDate) VALUES (3, '8', '2025-01-23 09:30:00');
INSERT INTO Registrations (EventId, UserId, RegistrationDate) VALUES (3, '4', '2025-01-24 15:20:00');
INSERT INTO Registrations (EventId, UserId, RegistrationDate) VALUES (3, '6', '2025-01-25 11:10:00');

-- Event 4: Database Design Workshop
INSERT INTO Registrations (EventId, UserId, RegistrationDate) VALUES (4, '1', '2025-01-26 14:50:00');
INSERT INTO Registrations (EventId, UserId, RegistrationDate) VALUES (4, '5', '2025-01-27 10:05:00');

-- Event 5: Mobile App Development Bootcamp
INSERT INTO Registrations (EventId, UserId, RegistrationDate) VALUES (5, '4', '2025-01-28 16:25:00');
INSERT INTO Registrations (EventId, UserId, RegistrationDate) VALUES (5, '6', '2025-01-29 09:40:00');

-- Event 6: Web Security Fundamentals
INSERT INTO Registrations (EventId, UserId, RegistrationDate) VALUES (6, '1', '2025-02-02 13:15:00');
INSERT INTO Registrations (EventId, UserId, RegistrationDate) VALUES (6, '2', '2025-02-03 10:30:00');
INSERT INTO Registrations (EventId, UserId, RegistrationDate) VALUES (6, '8', '2025-02-03 10:30:00');
INSERT INTO Registrations (EventId, UserId, RegistrationDate) VALUES (6, '4', '2025-02-04 15:45:00');
INSERT INTO Registrations (EventId, UserId, RegistrationDate) VALUES (6, '5', '2025-02-05 09:10:00');

-- Event 7: Introduction to ASP.NET MVC
INSERT INTO Registrations (EventId, UserId, RegistrationDate) VALUES (7, '1', '2025-01-12 08:15:00');
INSERT INTO Registrations (EventId, UserId, RegistrationDate) VALUES (7, '8', '2025-01-12 08:15:00');
INSERT INTO Registrations (EventId, UserId, RegistrationDate) VALUES (7, '4', '2025-01-13 12:30:00');
INSERT INTO Registrations (EventId, UserId, RegistrationDate) VALUES (7, '5', '2025-01-14 17:45:00');

-- Event 8: Annual Tech Summit
INSERT INTO Registrations (EventId, UserId, RegistrationDate) VALUES (8, '1', '2025-01-16 09:20:00');
INSERT INTO Registrations (EventId, UserId, RegistrationDate) VALUES (8, '3', '2025-01-17 14:10:00');
INSERT INTO Registrations (EventId, UserId, RegistrationDate) VALUES (8, '4', '2025-01-18 11:05:00');
INSERT INTO Registrations (EventId, UserId, RegistrationDate) VALUES (8, '5', '2025-01-19 16:30:00');
INSERT INTO Registrations (EventId, UserId, RegistrationDate) VALUES (8, '6', '2025-01-20 10:15:00');