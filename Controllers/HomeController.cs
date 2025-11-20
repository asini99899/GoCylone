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
            if (string.IsNullOrWhiteSpace(request?.FromLocation) || string.IsNullOrWhiteSpace(request?.ToLocation))
            {
                return BadRequest(new { success = false, message = "From and To locations are required" });
            }

            // Search for buses on the specified route and date
            var fromLoc = request.FromLocation.Trim();
            var toLoc = request.ToLocation.Trim();
            var searchDate = request.SearchDate;

            var buses = await _context.Schedules
                .Where(s => s.Route!.FromLocation.ToLower().Contains(fromLoc.ToLower()) &&
                           s.Route.ToLocation.ToLower().Contains(toLoc.ToLower()) &&
                           s.ScheduledDate.Date == searchDate.Date)
                .Include(s => s.Bus)
                .Include(s => s.Route)
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
                .ToListAsync();

            if (!buses.Any())
            {
                return Ok(new { success = true, data = buses, message = "No buses found for the selected route and date" });
            }

            return Ok(new { success = true, data = buses, message = $"Found {buses.Count} buses" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching buses");
            return BadRequest(new { success = false, message = "Error searching buses", error = ex.Message });
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
