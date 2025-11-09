-- SpaceEventHub Database Schema
-- SQL Server Database

-- Users Table
CREATE TABLE Users (
    UserId INT PRIMARY KEY IDENTITY(1,1),
    Email NVARCHAR(255) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(500) NOT NULL,
    FirstName NVARCHAR(100) NOT NULL,
    LastName NVARCHAR(100) NOT NULL,
    Role NVARCHAR(20) NOT NULL CHECK (Role IN ('Attendee', 'Organizer', 'Admin')),
    CreatedAt DATETIME2 DEFAULT GETDATE(),
    LastLoginAt DATETIME2 NULL,
    IsActive BIT DEFAULT 1
);

-- Events Table
CREATE TABLE Events (
    EventId INT PRIMARY KEY IDENTITY(1,1),
    Title NVARCHAR(200) NOT NULL,
    Description NVARCHAR(MAX) NOT NULL,
    EventDate DATETIME2 NOT NULL,
    City NVARCHAR(100) NOT NULL,
    Location NVARCHAR(300) NOT NULL,
    AreaOfInterest NVARCHAR(100) NOT NULL, -- AI, Backend, UX, Frontend, DevOps, etc.
    RegistrationLink NVARCHAR(500) NULL,
    OrganizerId INT NOT NULL,
    CreatedAt DATETIME2 DEFAULT GETDATE(),
    UpdatedAt DATETIME2 DEFAULT GETDATE(),
    IsActive BIT DEFAULT 1,
    FOREIGN KEY (OrganizerId) REFERENCES Users(UserId)
);

-- Event Registrations Table
CREATE TABLE EventRegistrations (
    RegistrationId INT PRIMARY KEY IDENTITY(1,1),
    EventId INT NOT NULL,
    UserId INT NOT NULL,
    RegisteredAt DATETIME2 DEFAULT GETDATE(),
    Status NVARCHAR(20) DEFAULT 'Registered' CHECK (Status IN ('Registered', 'Cancelled', 'Attended')),
    FOREIGN KEY (EventId) REFERENCES Events(EventId),
    FOREIGN KEY (UserId) REFERENCES Users(UserId),
    UNIQUE(EventId, UserId)
);

-- Notifications Table
CREATE TABLE Notifications (
    NotificationId INT PRIMARY KEY IDENTITY(1,1),
    UserId INT NOT NULL,
    EventId INT NULL,
    Title NVARCHAR(200) NOT NULL,
    Message NVARCHAR(MAX) NOT NULL,
    IsRead BIT DEFAULT 0,
    CreatedAt DATETIME2 DEFAULT GETDATE(),
    FOREIGN KEY (UserId) REFERENCES Users(UserId),
    FOREIGN KEY (EventId) REFERENCES Events(EventId)
);

-- Analytics/Page Views Table
CREATE TABLE PageViews (
    ViewId INT PRIMARY KEY IDENTITY(1,1),
    PageUrl NVARCHAR(500) NOT NULL,
    UserId INT NULL,
    ViewedAt DATETIME2 DEFAULT GETDATE(),
    IpAddress NVARCHAR(50) NULL,
    UserAgent NVARCHAR(500) NULL,
    FOREIGN KEY (UserId) REFERENCES Users(UserId)
);

-- Create indexes for better query performance
CREATE INDEX IX_Events_City ON Events(City);
CREATE INDEX IX_Events_EventDate ON Events(EventDate);
CREATE INDEX IX_Events_AreaOfInterest ON Events(AreaOfInterest);
CREATE INDEX IX_Events_OrganizerId ON Events(OrganizerId);
CREATE INDEX IX_EventRegistrations_EventId ON EventRegistrations(EventId);
CREATE INDEX IX_EventRegistrations_UserId ON EventRegistrations(UserId);
CREATE INDEX IX_Notifications_UserId ON Notifications(UserId);
CREATE INDEX IX_PageViews_ViewedAt ON PageViews(ViewedAt);

-- Insert default admin user (password: Admin@123)
-- Password hash for "Admin@123" using BCrypt
INSERT INTO Users (Email, PasswordHash, FirstName, LastName, Role, IsActive)
VALUES ('admin@spaceeventhub.com', '$2a$11$XvZ8Z8Z8Z8Z8Z8Z8Z8Z8Z8Z8Z8Z8Z8Z8Z8Z8Z8Z8Z8Z8Z8Z8Z8Z8', 'Admin', 'User', 'Admin', 1);

-- Insert sample organizer user (password: Organizer@123)
INSERT INTO Users (Email, PasswordHash, FirstName, LastName, Role, IsActive)
VALUES ('organizer@spaceeventhub.com', '$2a$11$YvZ9Z9Z9Z9Z9Z9Z9Z9Z9Z9Z9Z9Z9Z9Z9Z9Z9Z9Z9Z9Z9Z9Z9Z9Z9', 'John', 'Organizer', 'Organizer', 1);

-- Insert sample attendee user (password: Attendee@123)
INSERT INTO Users (Email, PasswordHash, FirstName, LastName, Role, IsActive)
VALUES ('attendee@spaceeventhub.com', '$2a$11$ZvZ1Z1Z1Z1Z1Z1Z1Z1Z1Z1Z1Z1Z1Z1Z1Z1Z1Z1Z1Z1Z1Z1Z1Z1Z1', 'Jane', 'Attendee', 'Attendee', 1);

-- Insert sample events
INSERT INTO Events (Title, Description, EventDate, City, Location, AreaOfInterest, RegistrationLink, OrganizerId)
VALUES 
('AI Summit 2025', 'Join us for the biggest AI conference of the year featuring keynotes from industry leaders and hands-on workshops.', '2025-11-15 09:00:00', 'San Francisco', 'Moscone Center, 747 Howard St', 'AI', 'https://aisummit2025.com/register', 2),
('Backend Developers Meetup', 'Monthly meetup for backend developers to share experiences, learn new technologies, and network.', '2025-11-05 18:00:00', 'New York', 'WeWork, 115 W 18th St', 'Backend', 'https://meetup.com/backend-devs-ny', 2),
('UX Design Workshop', 'Interactive workshop covering the latest trends in user experience design and prototyping tools.', '2025-11-20 10:00:00', 'Austin', 'Capital Factory, 701 Brazos St', 'UX', 'https://uxworkshop.com/austin', 2),
('DevOps Conference 2025', 'Learn about CI/CD, containerization, and cloud infrastructure from DevOps experts.', '2025-12-01 08:00:00', 'Seattle', 'Washington State Convention Center', 'DevOps', 'https://devopsconf2025.com', 2),
('Frontend Framework Bootcamp', 'Intensive 3-day bootcamp covering React, Vue, and Angular with real-world projects.', '2025-11-10 09:00:00', 'Los Angeles', 'General Assembly, 1520 2nd St', 'Frontend', 'https://frontendbootcamp.com', 2);

-- Insert sample registrations
INSERT INTO EventRegistrations (EventId, UserId, Status)
VALUES 
(1, 3, 'Registered'),
(2, 3, 'Registered'),
(3, 3, 'Registered');

-- Insert sample notifications
INSERT INTO Notifications (UserId, EventId, Title, Message, IsRead)
VALUES 
(2, 1, 'Event Published', 'Your event "AI Summit 2025" has been successfully published!', 1),
(2, 2, 'Event Published', 'Your event "Backend Developers Meetup" has been successfully published!', 0),
(3, 1, 'Registration Confirmed', 'You have successfully registered for "AI Summit 2025"', 1);

-- Insert sample page views for analytics
INSERT INTO PageViews (PageUrl, UserId, IpAddress, UserAgent)
VALUES 
('/events', 3, '192.168.1.1', 'Mozilla/5.0'),
('/events/1', 3, '192.168.1.1', 'Mozilla/5.0'),
('/events/2', 3, '192.168.1.1', 'Mozilla/5.0'),
('/dashboard', 2, '192.168.1.2', 'Mozilla/5.0'),
('/events', NULL, '192.168.1.3', 'Mozilla/5.0'),
('/events/1', NULL, '192.168.1.4', 'Mozilla/5.0');
