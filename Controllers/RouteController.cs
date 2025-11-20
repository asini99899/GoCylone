using GoCylone.Data;
using GoCylone.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GoCylone.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RouteController : ControllerBase
    {
        private readonly GoCyloneDbContext _context;
        private readonly ILogger<RouteController> _logger;

        public RouteController(GoCyloneDbContext context, ILogger<RouteController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/route
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Models.Route>>> GetAllRoutes()
        {
            try
            {
                var routes = await _context.Routes
                    .OrderByDescending(r => r.CreatedAt)
                    .ToListAsync();
                return Ok(new { success = true, data = routes, message = "Routes retrieved successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving routes");
                return BadRequest(new { success = false, message = "Error retrieving routes", error = ex.Message });
            }
        }

        // GET: api/route/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Models.Route>> GetRouteById(int id)
        {
            try
            {
                var route = await _context.Routes.FindAsync(id);
                if (route == null)
                    return NotFound(new { success = false, message = "Route not found" });

                return Ok(new { success = true, data = route, message = "Route retrieved successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving route");
                return BadRequest(new { success = false, message = "Error retrieving route", error = ex.Message });
            }
        }

        // POST: api/route
        [HttpPost]
        public async Task<ActionResult<Models.Route>> AddRoute([FromBody] Models.Route route)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new { success = false, message = "Invalid data", errors = ModelState });

                if (string.IsNullOrWhiteSpace(route.FromLocation) || string.IsNullOrWhiteSpace(route.ToLocation))
                    return BadRequest(new { success = false, message = "From and To locations are required" });

                if (route.Distance <= 0)
                    return BadRequest(new { success = false, message = "Distance must be greater than 0" });

                route.CreatedAt = DateTime.Now;
                _context.Routes.Add(route);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetRouteById), new { id = route.RouteId },
                    new { success = true, data = route, message = "Route added successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding route");
                return BadRequest(new { success = false, message = "Error adding route", error = ex.Message });
            }
        }

        // PUT: api/route/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRoute(int id, [FromBody] Models.Route route)
        {
            try
            {
                if (id != route.RouteId)
                    return BadRequest(new { success = false, message = "ID mismatch" });

                var existingRoute = await _context.Routes.FindAsync(id);
                if (existingRoute == null)
                    return NotFound(new { success = false, message = "Route not found" });

                if (string.IsNullOrWhiteSpace(route.FromLocation) || string.IsNullOrWhiteSpace(route.ToLocation))
                    return BadRequest(new { success = false, message = "From and To locations are required" });

                if (route.Distance <= 0)
                    return BadRequest(new { success = false, message = "Distance must be greater than 0" });

                existingRoute.FromLocation = route.FromLocation;
                existingRoute.ToLocation = route.ToLocation;
                existingRoute.Distance = route.Distance;
                existingRoute.EstimatedTime = route.EstimatedTime;
                existingRoute.UpdatedAt = DateTime.Now;

                _context.Routes.Update(existingRoute);
                await _context.SaveChangesAsync();

                return Ok(new { success = true, data = existingRoute, message = "Route updated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating route");
                return BadRequest(new { success = false, message = "Error updating route", error = ex.Message });
            }
        }

        // DELETE: api/route/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoute(int id)
        {
            try
            {
                var route = await _context.Routes.FindAsync(id);
                if (route == null)
                    return NotFound(new { success = false, message = "Route not found" });

                // Check if route has schedules
                var schedules = await _context.Schedules.Where(s => s.RouteId == id).ToListAsync();
                if (schedules.Any())
                    return BadRequest(new { success = false, message = "Cannot delete route with active schedules" });

                _context.Routes.Remove(route);
                await _context.SaveChangesAsync();

                return Ok(new { success = true, message = "Route deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting route");
                return BadRequest(new { success = false, message = "Error deleting route", error = ex.Message });
            }
        }
    }
}
