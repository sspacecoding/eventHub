using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpaceEventHub.API.Services;

namespace SpaceEventHub.API.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/[controller]")]
public class AnalyticsController : ControllerBase
{
    private readonly IAnalyticsService _analyticsService;

    public AnalyticsController(IAnalyticsService analyticsService)
    {
        _analyticsService = analyticsService;
    }

    [HttpGet("overview")]
    public async Task<IActionResult> GetOverview()
    {
        var overview = await _analyticsService.GetOverviewAsync();
        return Ok(overview);
    }

    [HttpGet("page-views")]
    public async Task<IActionResult> GetPageViews([FromQuery] int days = 7)
    {
        var stats = await _analyticsService.GetPageViewStatsAsync(days);
        return Ok(stats);
    }

    [HttpGet("registrations")]
    public async Task<IActionResult> GetRegistrations()
    {
        var stats = await _analyticsService.GetRegistrationStatsAsync();
        return Ok(stats);
    }

    [AllowAnonymous]
    [HttpPost("track")]
    public async Task<IActionResult> TrackPageView([FromBody] TrackPageViewRequest request)
    {
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();

        await _analyticsService.TrackPageViewAsync(request.PageUrl, request.UserId, ipAddress, userAgent);
        return Ok();
    }
}

public class TrackPageViewRequest
{
    public string PageUrl { get; set; } = string.Empty;
    public int? UserId { get; set; }
}
