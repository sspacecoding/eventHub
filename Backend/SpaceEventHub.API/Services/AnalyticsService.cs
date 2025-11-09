using Microsoft.EntityFrameworkCore;
using SpaceEventHub.API.Data;
using SpaceEventHub.API.DTOs;
using SpaceEventHub.API.Models;

namespace SpaceEventHub.API.Services;

public interface IAnalyticsService
{
    Task<AnalyticsOverviewDTO> GetOverviewAsync();
    Task<List<PageViewStatsDTO>> GetPageViewStatsAsync(int days = 7);
    Task<List<RegistrationStatsDTO>> GetRegistrationStatsAsync();
    Task TrackPageViewAsync(string pageUrl, int? userId, string? ipAddress, string? userAgent);
}

public class AnalyticsService : IAnalyticsService
{
    private readonly SpaceEventHubContext _context;

    public AnalyticsService(SpaceEventHubContext context)
    {
        _context = context;
    }

    public async Task<AnalyticsOverviewDTO> GetOverviewAsync()
    {
        var totalVisitors = await _context.PageViews
            .Select(pv => pv.IpAddress)
            .Distinct()
            .CountAsync();

        var totalPageViews = await _context.PageViews.CountAsync();

        var totalEvents = await _context.Events.CountAsync();

        var activeEvents = await _context.Events
            .Where(e => e.IsActive && e.EventDate >= DateTime.UtcNow)
            .CountAsync();

        var totalRegistrations = await _context.EventRegistrations.CountAsync();

        var totalUsers = await _context.Users
            .Where(u => u.IsActive)
            .CountAsync();

        return new AnalyticsOverviewDTO
        {
            TotalVisitors = totalVisitors,
            TotalPageViews = totalPageViews,
            TotalEvents = totalEvents,
            ActiveEvents = activeEvents,
            TotalRegistrations = totalRegistrations,
            TotalUsers = totalUsers
        };
    }

    public async Task<List<PageViewStatsDTO>> GetPageViewStatsAsync(int days = 7)
    {
        var startDate = DateTime.UtcNow.AddDays(-days);

        // Group by date in database
        var groupedData = await _context.PageViews
            .Where(pv => pv.ViewedAt >= startDate)
            .GroupBy(pv => pv.ViewedAt.Date)
            .Select(g => new
            {
                Date = g.Key,
                ViewCount = g.Count()
            })
            .OrderBy(s => s.Date)
            .ToListAsync();

        // Format date in memory after loading from database
        var stats = groupedData.Select(g => new PageViewStatsDTO
        {
            Date = g.Date.ToString("yyyy-MM-dd"),
            ViewCount = g.ViewCount
        }).ToList();

        return stats;
    }

    public async Task<List<RegistrationStatsDTO>> GetRegistrationStatsAsync()
    {
        var stats = await _context.EventRegistrations
            .Include(r => r.Event)
            .Where(r => r.Event!.IsActive)
            .GroupBy(r => r.Event!.Title)
            .Select(g => new RegistrationStatsDTO
            {
                EventTitle = g.Key,
                RegistrationCount = g.Count()
            })
            .OrderByDescending(s => s.RegistrationCount)
            .Take(10)
            .ToListAsync();

        return stats;
    }

    public async Task TrackPageViewAsync(string pageUrl, int? userId, string? ipAddress, string? userAgent)
    {
        var pageView = new PageView
        {
            PageUrl = pageUrl,
            UserId = userId,
            IpAddress = ipAddress,
            UserAgent = userAgent,
            ViewedAt = DateTime.UtcNow
        };

        _context.PageViews.Add(pageView);
        await _context.SaveChangesAsync();
    }
}
