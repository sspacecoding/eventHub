using Microsoft.EntityFrameworkCore;
using SpaceEventHub.API.Models;

namespace SpaceEventHub.API.Data;

public class SpaceEventHubContext : DbContext
{
    public SpaceEventHubContext(DbContextOptions<SpaceEventHubContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Event> Events { get; set; }
    public DbSet<EventRegistration> EventRegistrations { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<PageView> PageViews { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
        });

        // Event configuration
        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasKey(e => e.EventId);
            entity.HasIndex(e => e.City);
            entity.HasIndex(e => e.EventDate);
            entity.HasIndex(e => e.AreaOfInterest);
            entity.HasIndex(e => e.OrganizerId);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETDATE()");

            entity.HasOne(e => e.Organizer)
                .WithMany(u => u.OrganizedEvents)
                .HasForeignKey(e => e.OrganizerId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // EventRegistration configuration
        modelBuilder.Entity<EventRegistration>(entity =>
        {
            entity.HasKey(e => e.RegistrationId);
            entity.HasIndex(e => new { e.EventId, e.UserId }).IsUnique();
            entity.HasIndex(e => e.EventId);
            entity.HasIndex(e => e.UserId);
            entity.Property(e => e.RegisteredAt).HasDefaultValueSql("GETDATE()");

            entity.HasOne(er => er.Event)
                .WithMany(e => e.Registrations)
                .HasForeignKey(er => er.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(er => er.User)
                .WithMany(u => u.EventRegistrations)
                .HasForeignKey(er => er.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Notification configuration
        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId);
            entity.HasIndex(e => e.UserId);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");

            entity.HasOne(n => n.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(n => n.Event)
                .WithMany(e => e.Notifications)
                .HasForeignKey(n => n.EventId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // PageView configuration
        modelBuilder.Entity<PageView>(entity =>
        {
            entity.HasKey(e => e.ViewId);
            entity.HasIndex(e => e.ViewedAt);
            entity.Property(e => e.ViewedAt).HasDefaultValueSql("GETDATE()");

            entity.HasOne(pv => pv.User)
                .WithMany(u => u.PageViews)
                .HasForeignKey(pv => pv.UserId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Seed initial data
        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        // Seed admin user (password: Admin@123)
        modelBuilder.Entity<User>().HasData(
            new User
            {
                UserId = 1,
                Email = "admin@spaceeventhub.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                FirstName = "Admin",
                LastName = "User",
                Role = "Admin",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            }
        );

        // Seed organizer user (password: Organizer@123)
        modelBuilder.Entity<User>().HasData(
            new User
            {
                UserId = 2,
                Email = "organizer@spaceeventhub.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Organizer@123"),
                FirstName = "John",
                LastName = "Organizer",
                Role = "Organizer",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            }
        );

        // Seed attendee user (password: Attendee@123)
        modelBuilder.Entity<User>().HasData(
            new User
            {
                UserId = 3,
                Email = "attendee@spaceeventhub.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Attendee@123"),
                FirstName = "Jane",
                LastName = "Attendee",
                Role = "Attendee",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            }
        );

        // Seed sample events
        modelBuilder.Entity<Event>().HasData(
            new Event
            {
                EventId = 1,
                Title = "AI Summit 2025",
                Description = "Join us for the biggest AI conference of the year featuring keynotes from industry leaders and hands-on workshops.",
                EventDate = new DateTime(2025, 11, 15, 9, 0, 0),
                City = "San Francisco",
                Location = "Moscone Center, 747 Howard St",
                AreaOfInterest = "AI",
                RegistrationLink = "https://aisummit2025.com/register",
                OrganizerId = 2,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Event
            {
                EventId = 2,
                Title = "Backend Developers Meetup",
                Description = "Monthly meetup for backend developers to share experiences, learn new technologies, and network.",
                EventDate = new DateTime(2025, 11, 5, 18, 0, 0),
                City = "New York",
                Location = "WeWork, 115 W 18th St",
                AreaOfInterest = "Backend",
                RegistrationLink = "https://meetup.com/backend-devs-ny",
                OrganizerId = 2,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Event
            {
                EventId = 3,
                Title = "UX Design Workshop",
                Description = "Interactive workshop covering the latest trends in user experience design and prototyping tools.",
                EventDate = new DateTime(2025, 11, 20, 10, 0, 0),
                City = "Austin",
                Location = "Capital Factory, 701 Brazos St",
                AreaOfInterest = "UX",
                RegistrationLink = "https://uxworkshop.com/austin",
                OrganizerId = 2,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Event
            {
                EventId = 4,
                Title = "DevOps Conference 2025",
                Description = "Learn about CI/CD, containerization, and cloud infrastructure from DevOps experts.",
                EventDate = new DateTime(2025, 12, 1, 8, 0, 0),
                City = "Seattle",
                Location = "Washington State Convention Center",
                AreaOfInterest = "DevOps",
                RegistrationLink = "https://devopsconf2025.com",
                OrganizerId = 2,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Event
            {
                EventId = 5,
                Title = "Frontend Framework Bootcamp",
                Description = "Intensive 3-day bootcamp covering React, Vue, and Angular with real-world projects.",
                EventDate = new DateTime(2025, 11, 10, 9, 0, 0),
                City = "Los Angeles",
                Location = "General Assembly, 1520 2nd St",
                AreaOfInterest = "Frontend",
                RegistrationLink = "https://frontendbootcamp.com",
                OrganizerId = 2,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        );
    }
}
