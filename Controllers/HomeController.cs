using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using GoCylone.Models;
using GoCylone.Data;
using Microsoft.EntityFrameworkCore;

namespace GoCylone.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly GoCyloneDbContext _context;

    public HomeController(ILogger<HomeController> logger, GoCyloneDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public IActionResult Index()
    {
        // User is authenticated if they reach here (middleware handles auth check)
        var userId = HttpContext.Session.GetString("UserId");
        var fullName = HttpContext.Session.GetString("FullName");

        _logger.LogInformation($"User {userId} ({fullName}) accessing Home/Index");
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> SearchBuses([FromBody] SearchBusRequest request)
    {
        try
        {
            _logger.LogInformation($"SearchBuses called with: FromLocation={request?.FromLocation}, ToLocation={request?.ToLocation}, SearchDate={request?.SearchDate}");

            if (request == null || string.IsNullOrWhiteSpace(request.FromLocation) || string.IsNullOrWhiteSpace(request.ToLocation))
            {
                _logger.LogWarning("Invalid search parameters");
                return BadRequest(new { success = false, message = "From and To locations are required" });
            }

            // Search for buses on the specified route and date
            var fromLoc = request.FromLocation.Trim().ToLower();
            var toLoc = request.ToLocation.Trim().ToLower();
            var searchDate = request.SearchDate.Date;

            _logger.LogInformation($"Searching for buses: From={fromLoc}, To={toLoc}, Date={searchDate:yyyy-MM-dd}");

            // First, log all available routes for debugging
            var allRoutes = await _context.Routes.ToListAsync();
            _logger.LogInformation($"Total routes in DB: {allRoutes.Count}");
            foreach (var route in allRoutes)
            {
                _logger.LogInformation($"  Route: {route.FromLocation} -> {route.ToLocation}");
            }

            // Get all schedules for the search date
            var allSchedulesForDate = await _context.Schedules
                .Where(s => s.ScheduledDate.Date == searchDate)
                .Include(s => s.Bus)
                .Include(s => s.Route)
                .ToListAsync();

            _logger.LogInformation($"Total schedules for date {searchDate:yyyy-MM-dd}: {allSchedulesForDate.Count}");

            // Filter by route - use exact match first, then fallback to partial match
            var buses = allSchedulesForDate
                .Where(s =>
                    (s.Route!.FromLocation.ToLower() == fromLoc && s.Route.ToLocation.ToLower() == toLoc) ||
                    (s.Route!.FromLocation.ToLower().Contains(fromLoc) && s.Route.ToLocation.ToLower().Contains(toLoc))
                )
                .Select(s => new
                {
                    scheduleId = s.ScheduleId,
                    busId = s.Bus!.BusId,
                    numberPlate = s.Bus.NumberPlate,
                    numberOfSeats = s.Bus.NumberOfSeats,
                    seatStructure = s.Bus.SeatStructure,
                    condition = s.Bus.Condition,
                    fromLocation = s.Route!.FromLocation,
                    toLocation = s.Route.ToLocation,
                    departureTime = s.DepartureTime,
                    scheduledDate = s.ScheduledDate,
                    distance = s.Route.Distance,
                    estimatedTime = s.Route.EstimatedTime
                })
                .ToList();

            _logger.LogInformation($"Found {buses.Count} buses matching route and date");

            if (!buses.Any())
            {
                return Ok(new { success = true, data = buses, message = "No buses found for the selected route and date" });
            }

            return Ok(new { success = true, data = buses, message = $"Found {buses.Count} buses" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching buses");
            return BadRequest(new { success = false, message = "Error searching buses: " + ex.Message, error = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetLocations()
    {
        try
        {
            var fromLocations = await _context.Routes
                .Select(r => r.FromLocation)
                .Distinct()
                .ToListAsync();

            var toLocations = await _context.Routes
                .Select(r => r.ToLocation)
                .Distinct()
                .ToListAsync();

            var locations = fromLocations
                .Concat(toLocations)
                .Distinct()
                .OrderBy(l => l)
                .ToList();

            return Ok(new { success = true, data = locations });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving locations");
            return BadRequest(new { success = false, message = "Error retrieving locations", error = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> DebugData()
    {
        try
        {
            var routes = await _context.Routes.ToListAsync();
            var buses = await _context.Buses.ToListAsync();
            var schedules = await _context.Schedules
                .Include(s => s.Route)
                .Include(s => s.Bus)
                .ToListAsync();

            return Ok(new
            {
                success = true,
                routes = routes.Select(r => new { r.RouteId, r.FromLocation, r.ToLocation, r.Distance, r.EstimatedTime }),
                buses = buses.Select(b => new { b.BusId, b.NumberPlate, b.NumberOfSeats, b.SeatStructure, b.Condition }),
                schedules = schedules.Select(s => new
                {
                    s.ScheduleId,
                    s.ScheduledDate,
                    s.DepartureTime,
                    BusNumberPlate = s.Bus!.NumberPlate,
                    FromLocation = s.Route!.FromLocation,
                    ToLocation = s.Route.ToLocation
                })
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting debug data");
            return BadRequest(new { success = false, message = "Error getting debug data", error = ex.Message });
        }
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

// Search request model
public class SearchBusRequest
{
    public string FromLocation { get; set; } = string.Empty;
    public string ToLocation { get; set; } = string.Empty;
    public DateTime SearchDate { get; set; }
}
