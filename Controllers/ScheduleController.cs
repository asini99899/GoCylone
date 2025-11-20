using GoCylone.Data;
using GoCylone.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GoCylone.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ScheduleController : ControllerBase
    {
        private readonly GoCyloneDbContext _context;
        private readonly ILogger<ScheduleController> _logger;

        public ScheduleController(GoCyloneDbContext context, ILogger<ScheduleController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/schedule
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Schedule>>> GetAllSchedules()
        {
            try
            {
                var schedules = await _context.Schedules
                    .Include(s => s.Bus)
                    .Include(s => s.Route)
                    .OrderByDescending(s => s.ScheduledDate)
                    .ToListAsync();
                
                // Clear navigation properties to avoid circular references
                foreach (var schedule in schedules)
                {
                    if (schedule.Bus != null)
                    {
                        schedule.Bus.Schedules = new List<Schedule>();
                    }
                    if (schedule.Route != null)
                    {
                        schedule.Route.Schedules = new List<Schedule>();
                    }
                }
                
                return Ok(new { success = true, data = schedules, message = "Schedules retrieved successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving schedules");
                return BadRequest(new { success = false, message = "Error retrieving schedules", error = ex.Message });
            }
        }

        // GET: api/schedule/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Schedule>> GetScheduleById(int id)
        {
            try
            {
                var schedule = await _context.Schedules
                    .Include(s => s.Bus)
                    .Include(s => s.Route)
                    .FirstOrDefaultAsync(s => s.ScheduleId == id);

                if (schedule == null)
                    return NotFound(new { success = false, message = "Schedule not found" });

                // Clear navigation properties to avoid circular references
                if (schedule.Bus != null)
                    schedule.Bus.Schedules = new List<Schedule>();
                if (schedule.Route != null)
                    schedule.Route.Schedules = new List<Schedule>();

                return Ok(new { success = true, data = schedule, message = "Schedule retrieved successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving schedule");
                return BadRequest(new { success = false, message = "Error retrieving schedule", error = ex.Message });
            }
        }

        // POST: api/schedule
        [HttpPost]
        public async Task<ActionResult<Schedule>> AddSchedule([FromBody] Schedule schedule)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new { success = false, message = "Invalid data", errors = ModelState });

                // Validate bus exists
                var bus = await _context.Buses.FindAsync(schedule.BusId);
                if (bus == null)
                    return BadRequest(new { success = false, message = "Bus not found" });

                // Validate route exists
                var route = await _context.Routes.FindAsync(schedule.RouteId);
                if (route == null)
                    return BadRequest(new { success = false, message = "Route not found" });

                // Validate scheduled date is not in the past
                if (schedule.ScheduledDate.Date < DateTime.Now.Date)
                    return BadRequest(new { success = false, message = "Scheduled date cannot be in the past" });

                schedule.CreatedAt = DateTime.Now;
                _context.Schedules.Add(schedule);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetScheduleById), new { id = schedule.ScheduleId },
                    new { success = true, data = schedule, message = "Schedule added successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding schedule");
                return BadRequest(new { success = false, message = "Error adding schedule", error = ex.Message });
            }
        }

        // PUT: api/schedule/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSchedule(int id, [FromBody] Schedule schedule)
        {
            try
            {
                if (id != schedule.ScheduleId)
                    return BadRequest(new { success = false, message = "ID mismatch" });

                var existingSchedule = await _context.Schedules.FindAsync(id);
                if (existingSchedule == null)
                    return NotFound(new { success = false, message = "Schedule not found" });

                // Validate bus exists
                var bus = await _context.Buses.FindAsync(schedule.BusId);
                if (bus == null)
                    return BadRequest(new { success = false, message = "Bus not found" });

                // Validate route exists
                var route = await _context.Routes.FindAsync(schedule.RouteId);
                if (route == null)
                    return BadRequest(new { success = false, message = "Route not found" });

                // Validate scheduled date is not in the past
                if (schedule.ScheduledDate.Date < DateTime.Now.Date)
                    return BadRequest(new { success = false, message = "Scheduled date cannot be in the past" });

                existingSchedule.BusId = schedule.BusId;
                existingSchedule.RouteId = schedule.RouteId;
                existingSchedule.ScheduledDate = schedule.ScheduledDate;
                existingSchedule.DepartureTime = schedule.DepartureTime;
                existingSchedule.UpdatedAt = DateTime.Now;

                _context.Schedules.Update(existingSchedule);
                await _context.SaveChangesAsync();

                return Ok(new { success = true, data = existingSchedule, message = "Schedule updated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating schedule");
                return BadRequest(new { success = false, message = "Error updating schedule", error = ex.Message });
            }
        }

        // DELETE: api/schedule/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSchedule(int id)
        {
            try
            {
                var schedule = await _context.Schedules.FindAsync(id);
                if (schedule == null)
                    return NotFound(new { success = false, message = "Schedule not found" });

                _context.Schedules.Remove(schedule);
                await _context.SaveChangesAsync();

                return Ok(new { success = true, message = "Schedule deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting schedule");
                return BadRequest(new { success = false, message = "Error deleting schedule", error = ex.Message });
            }
        }

        // GET: api/schedule/bus/5 - Get schedules for a specific bus
        [HttpGet("bus/{busId}")]
        public async Task<ActionResult<IEnumerable<Schedule>>> GetSchedulesByBus(int busId)
        {
            try
            {
                var schedules = await _context.Schedules
                    .Where(s => s.BusId == busId)
                    .Include(s => s.Route)
                    .OrderByDescending(s => s.ScheduledDate)
                    .ToListAsync();

                foreach (var schedule in schedules)
                {
                    if (schedule.Route != null)
                        schedule.Route.Schedules = new List<Schedule>();
                }

                return Ok(new { success = true, data = schedules, message = "Schedules retrieved successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving schedules");
                return BadRequest(new { success = false, message = "Error retrieving schedules", error = ex.Message });
            }
        }

        // GET: api/schedule/route/5 - Get schedules for a specific route
        [HttpGet("route/{routeId}")]
        public async Task<ActionResult<IEnumerable<Schedule>>> GetSchedulesByRoute(int routeId)
        {
            try
            {
                var schedules = await _context.Schedules
                    .Where(s => s.RouteId == routeId)
                    .Include(s => s.Bus)
                    .OrderByDescending(s => s.ScheduledDate)
                    .ToListAsync();

                foreach (var schedule in schedules)
                {
                    if (schedule.Bus != null)
                        schedule.Bus.Schedules = new List<Schedule>();
                }

                return Ok(new { success = true, data = schedules, message = "Schedules retrieved successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving schedules");
                return BadRequest(new { success = false, message = "Error retrieving schedules", error = ex.Message });
            }
        }
    }
}
