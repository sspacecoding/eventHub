-- SpaceEventHub - Complete Seed Data
-- Execute this script to populate all tables with sample data

USE SpaceEventHubDB;
GO

-- Clear existing data
DELETE FROM PageViews;
DELETE FROM Notifications;
DELETE FROM EventRegistrations;
DELETE FROM Events;
DELETE FROM Users;
GO

-- Insert Users
-- Note: Password hashes are placeholders. In production, use BCrypt.Net.BCrypt.HashPassword()
-- For demo purposes, you can register new users via the API which will generate proper hashes
INSERT INTO Users (Email, PasswordHash, FirstName, LastName, Role, IsActive, CreatedAt)
VALUES 
('admin@spaceeventhub.com', '$2a$11$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LewY5qT5R5Q6q5Q6q', 'Admin', 'User', 'Admin', 1, GETDATE()),
('organizer@spaceeventhub.com', '$2a$11$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LewY5qT5R5Q6q5Q6q', 'John', 'Organizer', 'Organizer', 1, GETDATE()),
('attendee@spaceeventhub.com', '$2a$11$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LewY5qT5R5Q6q5Q6q', 'Jane', 'Attendee', 'Attendee', 1, GETDATE()),
('maria.silva@email.com', '$2a$11$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LewY5qT5R5Q6q5Q6q', 'Maria', 'Silva', 'Attendee', 1, GETDATE()),
('pedro.costa@email.com', '$2a$11$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LewY5qT5R5Q6q5Q6q', 'Pedro', 'Costa', 'Organizer', 1, GETDATE()),
('ana.santos@email.com', '$2a$11$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LewY5qT5R5Q6q5Q6q', 'Ana', 'Santos', 'Attendee', 1, GETDATE());
GO

-- Insert Events
INSERT INTO Events (Title, Description, EventDate, City, Location, AreaOfInterest, RegistrationLink, OrganizerId, IsActive, CreatedAt, UpdatedAt)
SELECT 
    'AI Summit 2025',
    'Join us for the biggest AI conference of the year featuring keynotes from industry leaders and hands-on workshops. Explore the latest in machine learning, deep learning, and AI applications.',
    '2025-11-15 09:00:00',
    'San Francisco',
    'Moscone Center, 747 Howard St',
    'AI',
    'https://aisummit2025.com/register',
    UserId,
    1,
    GETDATE(),
    GETDATE()
FROM Users WHERE Email = 'organizer@spaceeventhub.com'
UNION ALL
SELECT 'Backend Developers Meetup', 'Monthly meetup for backend developers to share experiences, learn new technologies, and network.', '2025-11-05 18:00:00', 'New York', 'WeWork, 115 W 18th St', 'Backend', 'https://meetup.com/backend-devs-ny', UserId, 1, GETDATE(), GETDATE() FROM Users WHERE Email = 'organizer@spaceeventhub.com'
UNION ALL
SELECT 'UX Design Workshop', 'Interactive workshop covering the latest trends in user experience design.', '2025-11-20 10:00:00', 'Austin', 'Capital Factory, 701 Brazos St', 'UX', 'https://uxworkshop.com/austin', UserId, 1, GETDATE(), GETDATE() FROM Users WHERE Email = 'pedro.costa@email.com'
UNION ALL
SELECT 'DevOps Conference 2025', 'Learn about CI/CD, containerization, and cloud infrastructure.', '2025-12-01 08:00:00', 'Seattle', 'Washington State Convention Center', 'DevOps', 'https://devopsconf2025.com', UserId, 1, GETDATE(), GETDATE() FROM Users WHERE Email = 'organizer@spaceeventhub.com'
UNION ALL
SELECT 'Frontend Framework Bootcamp', 'Intensive 3-day bootcamp covering React, Vue, and Angular.', '2025-11-10 09:00:00', 'Los Angeles', 'General Assembly, 1520 2nd St', 'Frontend', 'https://frontendbootcamp.com', UserId, 1, GETDATE(), GETDATE() FROM Users WHERE Email = 'organizer@spaceeventhub.com'
UNION ALL
SELECT 'Mobile Development Masterclass', 'Learn iOS and Android development with Swift, Kotlin, and React Native.', '2025-11-25 14:00:00', 'Chicago', '1871, 222 W Merchandise Mart Plaza', 'Mobile', 'https://mobilemasterclass.com', UserId, 1, GETDATE(), GETDATE() FROM Users WHERE Email = 'pedro.costa@email.com';
GO

-- Insert Event Registrations
INSERT INTO EventRegistrations (EventId, UserId, Status, RegisteredAt)
SELECT e.EventId, u.UserId, 'Registered', DATEADD(day, -5, GETDATE())
FROM Events e
CROSS JOIN Users u
WHERE (e.EventId = 1 AND u.Email = 'attendee@spaceeventhub.com')
   OR (e.EventId = 1 AND u.Email = 'maria.silva@email.com')
   OR (e.EventId = 2 AND u.Email = 'attendee@spaceeventhub.com')
   OR (e.EventId = 2 AND u.Email = 'maria.silva@email.com')
   OR (e.EventId = 3 AND u.Email = 'attendee@spaceeventhub.com')
   OR (e.EventId = 4 AND u.Email = 'maria.silva@email.com')
   OR (e.EventId = 5 AND u.Email = 'ana.santos@email.com')
   OR (e.EventId = 6 AND u.Email = 'ana.santos@email.com');
GO

-- Insert Notifications
INSERT INTO Notifications (UserId, EventId, Title, Message, IsRead, CreatedAt)
SELECT u.UserId, e.EventId, 'Event Published', 'Your event "' + e.Title + '" has been successfully published!', 1, DATEADD(day, -7, GETDATE())
FROM Users u
CROSS JOIN Events e
WHERE (u.Email = 'organizer@spaceeventhub.com' AND e.EventId IN (1, 2, 4, 5))
   OR (u.Email = 'pedro.costa@email.com' AND e.EventId IN (3, 6))
UNION ALL
SELECT u.UserId, e.EventId, 'Registration Confirmed', 'You have successfully registered for "' + e.Title + '".', 1, DATEADD(day, -3, GETDATE())
FROM Users u
CROSS JOIN Events e
WHERE (u.Email = 'attendee@spaceeventhub.com' AND e.EventId = 1)
   OR (u.Email = 'maria.silva@email.com' AND e.EventId = 1)
   OR (u.Email = 'ana.santos@email.com' AND e.EventId = 5);
GO

-- Insert Page Views
INSERT INTO PageViews (PageUrl, UserId, IpAddress, UserAgent, ViewedAt)
VALUES 
('/', NULL, '192.168.1.100', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64)', DATEADD(hour, -2, GETDATE())),
('/events', NULL, '192.168.1.101', 'Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7)', DATEADD(hour, -1, GETDATE())),
('/events', (SELECT UserId FROM Users WHERE Email = 'attendee@spaceeventhub.com'), '192.168.1.102', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64)', DATEADD(minute, -30, GETDATE())),
('/events/1', (SELECT UserId FROM Users WHERE Email = 'attendee@spaceeventhub.com'), '192.168.1.102', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64)', DATEADD(minute, -25, GETDATE())),
('/events/2', (SELECT UserId FROM Users WHERE Email = 'attendee@spaceeventhub.com'), '192.168.1.102', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64)', DATEADD(minute, -20, GETDATE())),
('/dashboard', (SELECT UserId FROM Users WHERE Email = 'organizer@spaceeventhub.com'), '192.168.1.103', 'Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7)', DATEADD(hour, -3, GETDATE())),
('/events', (SELECT UserId FROM Users WHERE Email = 'maria.silva@email.com'), '192.168.1.104', 'Mozilla/5.0 (iPhone; CPU iPhone OS 14_0)', DATEADD(minute, -15, GETDATE())),
('/events/1', (SELECT UserId FROM Users WHERE Email = 'maria.silva@email.com'), '192.168.1.104', 'Mozilla/5.0 (iPhone; CPU iPhone OS 14_0)', DATEADD(minute, -10, GETDATE())),
('/events/4', (SELECT UserId FROM Users WHERE Email = 'maria.silva@email.com'), '192.168.1.104', 'Mozilla/5.0 (iPhone; CPU iPhone OS 14_0)', DATEADD(minute, -5, GETDATE())),
('/dashboard', (SELECT UserId FROM Users WHERE Email = 'pedro.costa@email.com'), '192.168.1.105', 'Mozilla/5.0 (X11; Linux x86_64)', DATEADD(hour, -4, GETDATE())),
('/events/3', NULL, '192.168.1.106', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) Chrome/120.0.0.0', DATEADD(minute, -45, GETDATE())),
('/events/5', (SELECT UserId FROM Users WHERE Email = 'ana.santos@email.com'), '192.168.1.107', 'Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) Safari/605.1.15', DATEADD(hour, -6, GETDATE())),
('/events/6', (SELECT UserId FROM Users WHERE Email = 'ana.santos@email.com'), '192.168.1.107', 'Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) Safari/605.1.15', DATEADD(hour, -5, GETDATE())),
('/', NULL, '192.168.1.108', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) Edge/120.0.0.0', GETDATE()),
('/events', NULL, '192.168.1.109', 'Mozilla/5.0 (Android 11; Mobile)', DATEADD(minute, -10, GETDATE()));
GO

PRINT 'Seed data inserted successfully!';
PRINT 'Users: ' + CAST((SELECT COUNT(*) FROM Users) AS VARCHAR);
PRINT 'Events: ' + CAST((SELECT COUNT(*) FROM Events) AS VARCHAR);
PRINT 'Registrations: ' + CAST((SELECT COUNT(*) FROM EventRegistrations) AS VARCHAR);
PRINT 'Notifications: ' + CAST((SELECT COUNT(*) FROM Notifications) AS VARCHAR);
PRINT 'Page Views: ' + CAST((SELECT COUNT(*) FROM PageViews) AS VARCHAR);
GO

