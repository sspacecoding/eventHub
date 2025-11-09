-- SpaceEventHub - Seed Data Script
-- Insert comprehensive sample data into all tables

USE SpaceEventHubDB;
GO

-- Clear existing data (optional - comment out if you want to keep existing data)
DELETE FROM PageViews;
DELETE FROM Notifications;
DELETE FROM EventRegistrations;
DELETE FROM Events;
DELETE FROM Users;
GO

-- Insert Users with BCrypt password hashes
-- Note: These are sample BCrypt hashes. In production, use BCrypt.Net.BCrypt.HashPassword() in C#
-- Password for all users: Admin@123, Organizer@123, Attendee@123 (same password for simplicity in demo)
-- Real BCrypt hash for "Test123!" would be generated at runtime, but for demo we'll use a placeholder pattern
INSERT INTO Users (Email, PasswordHash, FirstName, LastName, Role, IsActive, CreatedAt)
VALUES 
('admin@spaceeventhub.com', '$2a$11$rOzJqX5YqX5YqX5YqX5YqeX5YqX5YqX5YqX5YqX5YqX5YqX5YqX5Yq', 'Admin', 'User', 'Admin', 1, GETDATE()),
('organizer@spaceeventhub.com', '$2a$11$rOzJqX5YqX5YqX5YqX5YqeX5YqX5YqX5YqX5YqX5YqX5YqX5YqX5Yq', 'John', 'Organizer', 'Organizer', 1, GETDATE()),
('attendee@spaceeventhub.com', '$2a$11$rOzJqX5YqX5YqX5YqX5YqeX5YqX5YqX5YqX5YqX5YqX5YqX5YqX5Yq', 'Jane', 'Attendee', 'Attendee', 1, GETDATE()),
('maria.silva@email.com', '$2a$11$rOzJqX5YqX5YqX5YqX5YqeX5YqX5YqX5YqX5YqX5YqX5YqX5YqX5Yq', 'Maria', 'Silva', 'Attendee', 1, GETDATE()),
('pedro.costa@email.com', '$2a$11$rOzJqX5YqX5YqX5YqX5YqeX5YqX5YqX5YqX5YqX5YqX5YqX5YqX5Yq', 'Pedro', 'Costa', 'Organizer', 1, GETDATE()),
('ana.santos@email.com', '$2a$11$rOzJqX5YqX5YqX5YqX5YqeX5YqX5YqX5YqX5YqX5YqX5YqX5YqX5Yq', 'Ana', 'Santos', 'Attendee', 1, GETDATE());
GO

-- Get User IDs for foreign keys
DECLARE @AdminId INT = (SELECT UserId FROM Users WHERE Email = 'admin@spaceeventhub.com');
DECLARE @OrganizerId INT = (SELECT UserId FROM Users WHERE Email = 'organizer@spaceeventhub.com');
DECLARE @AttendeeId INT = (SELECT UserId FROM Users WHERE Email = 'attendee@spaceeventhub.com');
DECLARE @MariaId INT = (SELECT UserId FROM Users WHERE Email = 'maria.silva@email.com');
DECLARE @PedroId INT = (SELECT UserId FROM Users WHERE Email = 'pedro.costa@email.com');
DECLARE @AnaId INT = (SELECT UserId FROM Users WHERE Email = 'ana.santos@email.com');
GO

-- Insert Events
INSERT INTO Events (Title, Description, EventDate, City, Location, AreaOfInterest, RegistrationLink, OrganizerId, IsActive, CreatedAt, UpdatedAt)
VALUES 
('AI Summit 2025', 'Join us for the biggest AI conference of the year featuring keynotes from industry leaders and hands-on workshops. Explore the latest in machine learning, deep learning, and AI applications.', '2025-11-15 09:00:00', 'San Francisco', 'Moscone Center, 747 Howard St', 'AI', 'https://aisummit2025.com/register', @OrganizerId, 1, GETDATE(), GETDATE()),

('Backend Developers Meetup', 'Monthly meetup for backend developers to share experiences, learn new technologies, and network. This month we will discuss microservices architecture and best practices.', '2025-11-05 18:00:00', 'New York', 'WeWork, 115 W 18th St', 'Backend', 'https://meetup.com/backend-devs-ny', @OrganizerId, 1, GETDATE(), GETDATE()),

('UX Design Workshop', 'Interactive workshop covering the latest trends in user experience design and prototyping tools. Learn from industry experts and practice with real-world scenarios.', '2025-11-20 10:00:00', 'Austin', 'Capital Factory, 701 Brazos St', 'UX', 'https://uxworkshop.com/austin', @PedroId, 1, GETDATE(), GETDATE()),

('DevOps Conference 2025', 'Learn about CI/CD, containerization, and cloud infrastructure from DevOps experts. Hands-on labs and case studies from leading tech companies.', '2025-12-01 08:00:00', 'Seattle', 'Washington State Convention Center', 'DevOps', 'https://devopsconf2025.com', @OrganizerId, 1, GETDATE(), GETDATE()),

('Frontend Framework Bootcamp', 'Intensive 3-day bootcamp covering React, Vue, and Angular with real-world projects. Perfect for developers looking to master modern frontend technologies.', '2025-11-10 09:00:00', 'Los Angeles', 'General Assembly, 1520 2nd St', 'Frontend', 'https://frontendbootcamp.com', @OrganizerId, 1, GETDATE(), GETDATE()),

('Mobile Development Masterclass', 'Learn iOS and Android development with Swift, Kotlin, and React Native. Build production-ready mobile apps from scratch.', '2025-11-25 14:00:00', 'Chicago', '1871, 222 W Merchandise Mart Plaza', 'Mobile', 'https://mobilemasterclass.com', @PedroId, 1, GETDATE(), GETDATE()),

('Data Science Summit', 'Discover the power of data analytics, machine learning, and artificial intelligence. Network with data scientists and learn cutting-edge techniques.', '2025-12-10 09:00:00', 'Boston', 'Boston Convention Center', 'Data Science', 'https://datascience2025.com', @OrganizerId, 1, GETDATE(), GETDATE()),

('Cloud Architecture Workshop', 'Deep dive into cloud computing, AWS, Azure, and GCP. Learn to design scalable and resilient cloud architectures.', '2025-11-18 10:00:00', 'Miami', 'Wynwood Warehouse, 2520 NW 2nd Ave', 'DevOps', 'https://cloudworkshop.com', @PedroId, 1, GETDATE(), GETDATE());
GO

-- Get Event IDs
DECLARE @Event1Id INT = (SELECT EventId FROM Events WHERE Title = 'AI Summit 2025');
DECLARE @Event2Id INT = (SELECT EventId FROM Events WHERE Title = 'Backend Developers Meetup');
DECLARE @Event3Id INT = (SELECT EventId FROM Events WHERE Title = 'UX Design Workshop');
DECLARE @Event4Id INT = (SELECT EventId FROM Events WHERE Title = 'DevOps Conference 2025');
DECLARE @Event5Id INT = (SELECT EventId FROM Events WHERE Title = 'Frontend Framework Bootcamp');
DECLARE @Event6Id INT = (SELECT EventId FROM Events WHERE Title = 'Mobile Development Masterclass');
GO

-- Insert Event Registrations
INSERT INTO EventRegistrations (EventId, UserId, Status, RegisteredAt)
VALUES 
(@Event1Id, @AttendeeId, 'Registered', DATEADD(day, -5, GETDATE())),
(@Event2Id, @AttendeeId, 'Registered', DATEADD(day, -3, GETDATE())),
(@Event3Id, @AttendeeId, 'Registered', DATEADD(day, -2, GETDATE())),
(@Event1Id, @MariaId, 'Registered', DATEADD(day, -4, GETDATE())),
(@Event4Id, @MariaId, 'Registered', DATEADD(day, -1, GETDATE())),
(@Event5Id, @AnaId, 'Registered', DATEADD(day, -6, GETDATE())),
(@Event6Id, @AnaId, 'Registered', DATEADD(day, -3, GETDATE())),
(@Event2Id, @MariaId, 'Registered', DATEADD(day, -2, GETDATE()));
GO

-- Insert Notifications
INSERT INTO Notifications (UserId, EventId, Title, Message, IsRead, CreatedAt)
VALUES 
(@OrganizerId, @Event1Id, 'Event Published', 'Your event "AI Summit 2025" has been successfully published and is now visible to attendees!', 1, DATEADD(day, -7, GETDATE())),
(@OrganizerId, @Event2Id, 'New Registration', 'Jane Attendee has registered for "Backend Developers Meetup"', 0, DATEADD(day, -3, GETDATE())),
(@OrganizerId, @Event2Id, 'New Registration', 'Maria Silva has registered for "Backend Developers Meetup"', 0, DATEADD(day, -2, GETDATE())),
(@AttendeeId, @Event1Id, 'Registration Confirmed', 'You have successfully registered for "AI Summit 2025". See you on November 15th!', 1, DATEADD(day, -5, GETDATE())),
(@AttendeeId, @Event2Id, 'Event Reminder', 'Reminder: "Backend Developers Meetup" is happening tomorrow at 6 PM!', 0, DATEADD(day, -1, GETDATE())),
(@AttendeeId, @Event3Id, 'Registration Confirmed', 'Your registration for "UX Design Workshop" has been confirmed.', 1, DATEADD(day, -2, GETDATE())),
(@MariaId, @Event1Id, 'Registration Confirmed', 'You have successfully registered for "AI Summit 2025".', 1, DATEADD(day, -4, GETDATE())),
(@MariaId, @Event4Id, 'Event Reminder', 'Don''t forget: "DevOps Conference 2025" starts in 2 weeks!', 0, GETDATE()),
(@PedroId, @Event3Id, 'Event Published', 'Your event "UX Design Workshop" has been published!', 1, DATEADD(day, -8, GETDATE())),
(@PedroId, @Event6Id, 'New Registration', 'Ana Santos has registered for "Mobile Development Masterclass"', 0, DATEADD(day, -3, GETDATE())),
(@AnaId, @Event5Id, 'Registration Confirmed', 'You have successfully registered for "Frontend Framework Bootcamp".', 1, DATEADD(day, -6, GETDATE())),
(@AnaId, @Event6Id, 'Registration Confirmed', 'Your registration for "Mobile Development Masterclass" has been confirmed.', 1, DATEADD(day, -3, GETDATE()));
GO

-- Insert Page Views for Analytics
INSERT INTO PageViews (PageUrl, UserId, IpAddress, UserAgent, ViewedAt)
VALUES 
('/', NULL, '192.168.1.100', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36', DATEADD(hour, -2, GETDATE())),
('/events', NULL, '192.168.1.101', 'Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36', DATEADD(hour, -1, GETDATE())),
('/events', @AttendeeId, '192.168.1.102', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36', DATEADD(minute, -30, GETDATE())),
('/events/1', @AttendeeId, '192.168.1.102', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36', DATEADD(minute, -25, GETDATE())),
('/events/2', @AttendeeId, '192.168.1.102', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36', DATEADD(minute, -20, GETDATE())),
('/dashboard', @OrganizerId, '192.168.1.103', 'Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36', DATEADD(hour, -3, GETDATE())),
('/events', @MariaId, '192.168.1.104', 'Mozilla/5.0 (iPhone; CPU iPhone OS 14_0 like Mac OS X) AppleWebKit/605.1.15', DATEADD(minute, -15, GETDATE())),
('/events/1', @MariaId, '192.168.1.104', 'Mozilla/5.0 (iPhone; CPU iPhone OS 14_0 like Mac OS X) AppleWebKit/605.1.15', DATEADD(minute, -10, GETDATE())),
('/events/4', @MariaId, '192.168.1.104', 'Mozilla/5.0 (iPhone; CPU iPhone OS 14_0 like Mac OS X) AppleWebKit/605.1.15', DATEADD(minute, -5, GETDATE())),
('/dashboard', @PedroId, '192.168.1.105', 'Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36', DATEADD(hour, -4, GETDATE())),
('/events/3', NULL, '192.168.1.106', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) Chrome/120.0.0.0', DATEADD(minute, -45, GETDATE())),
('/events/5', @AnaId, '192.168.1.107', 'Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) Safari/605.1.15', DATEADD(hour, -6, GETDATE())),
('/events/6', @AnaId, '192.168.1.107', 'Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) Safari/605.1.15', DATEADD(hour, -5, GETDATE())),
('/', NULL, '192.168.1.108', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) Edge/120.0.0.0', GETDATE()),
('/events', NULL, '192.168.1.109', 'Mozilla/5.0 (Android 11; Mobile) AppleWebKit/537.36', DATEADD(minute, -10, GETDATE()));
GO

PRINT 'Seed data inserted successfully!';
PRINT 'Users: ' + CAST((SELECT COUNT(*) FROM Users) AS VARCHAR);
PRINT 'Events: ' + CAST((SELECT COUNT(*) FROM Events) AS VARCHAR);
PRINT 'Registrations: ' + CAST((SELECT COUNT(*) FROM EventRegistrations) AS VARCHAR);
PRINT 'Notifications: ' + CAST((SELECT COUNT(*) FROM Notifications) AS VARCHAR);
PRINT 'Page Views: ' + CAST((SELECT COUNT(*) FROM PageViews) AS VARCHAR);
GO
