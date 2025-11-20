using GoCylone.Data;
using GoCylone.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GoCylone.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BusController : ControllerBase
    {
        private readonly GoCyloneDbContext _context;
        private readonly ILogger<BusController> _logger;

        public BusController(GoCyloneDbContext context, ILogger<BusController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/bus
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Bus>>> GetAllBuses()
        {
            try
            {
                var buses = await _context.Buses
                    .OrderByDescending(b => b.CreatedAt)
                    .ToListAsync();
                return Ok(new { success = true, data = buses, message = "Buses retrieved successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving buses");
                return BadRequest(new { success = false, message = "Error retrieving buses", error = ex.Message });
            }
        }

        // GET: api/bus/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Bus>> GetBusById(int id)
        {
            try
            {
                var bus = await _context.Buses.FindAsync(id);
                if (bus == null)
                    return NotFound(new { success = false, message = "Bus not found" });

                return Ok(new { success = true, data = bus, message = "Bus retrieved successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving bus");
                return BadRequest(new { success = false, message = "Error retrieving bus", error = ex.Message });
            }
        }

        // POST: api/bus
        [HttpPost]
        public async Task<ActionResult<Bus>> AddBus([FromBody] Bus bus)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new { success = false, message = "Invalid data", errors = ModelState });

                // Check if number plate already exists
                var existingBus = await _context.Buses
                    .FirstOrDefaultAsync(b => b.NumberPlate == bus.NumberPlate);
                if (existingBus != null)
                    return BadRequest(new { success = false, message = "Bus with this number plate already exists" });

                bus.CreatedAt = DateTime.Now;
                _context.Buses.Add(bus);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetBusById), new { id = bus.BusId },
                    new { success = true, data = bus, message = "Bus added successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding bus");
                return BadRequest(new { success = false, message = "Error adding bus", error = ex.Message });
            }
        }

        // PUT: api/bus/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBus(int id, [FromBody] Bus bus)
        {
            try
            {
                if (id != bus.BusId)
                    return BadRequest(new { success = false, message = "ID mismatch" });

                var existingBus = await _context.Buses.FindAsync(id);
                if (existingBus == null)
                    return NotFound(new { success = false, message = "Bus not found" });

                // Check if number plate is already used by another bus
                var duplicatePlate = await _context.Buses
                    .FirstOrDefaultAsync(b => b.NumberPlate == bus.NumberPlate && b.BusId != id);
                if (duplicatePlate != null)
                    return BadRequest(new { success = false, message = "This number plate is already in use" });

                existingBus.NumberPlate = bus.NumberPlate;
                existingBus.NumberOfSeats = bus.NumberOfSeats;
                existingBus.SeatStructure = bus.SeatStructure;
                existingBus.ConductorNumber = bus.ConductorNumber;
                existingBus.Condition = bus.Condition;
                existingBus.UpdatedAt = DateTime.Now;

                _context.Buses.Update(existingBus);
                await _context.SaveChangesAsync();

                return Ok(new { success = true, data = existingBus, message = "Bus updated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating bus");
                return BadRequest(new { success = false, message = "Error updating bus", error = ex.Message });
            }
        }

        // DELETE: api/bus/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBus(int id)
        {
            try
            {
                var bus = await _context.Buses.FindAsync(id);
                if (bus == null)
                    return NotFound(new { success = false, message = "Bus not found" });

                // Check if bus has schedules
                var schedules = await _context.Schedules.Where(s => s.BusId == id).ToListAsync();
                if (schedules.Any())
                    return BadRequest(new { success = false, message = "Cannot delete bus with active schedules" });

                _context.Buses.Remove(bus);
                await _context.SaveChangesAsync();

                return Ok(new { success = true, message = "Bus deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting bus");
                return BadRequest(new { success = false, message = "Error deleting bus", error = ex.Message });
            }
        }
    }
}
