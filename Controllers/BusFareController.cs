using GoCylone.Data;
using GoCylone.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GoCylone.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BusFareController : ControllerBase
    {
        private readonly GoCyloneDbContext _context;
        private readonly ILogger<BusFareController> _logger;

        public BusFareController(GoCyloneDbContext context, ILogger<BusFareController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/busfare
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BusFare>>> GetAllFares()
        {
            try
            {
                var fares = await _context.BusFares
                    .OrderByDescending(f => f.CreatedAt)
                    .ToListAsync();
                return Ok(new { success = true, data = fares, message = "Fares retrieved successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving fares");
                return BadRequest(new { success = false, message = "Error retrieving fares", error = ex.Message });
            }
        }

        // GET: api/busfare/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BusFare>> GetFareById(int id)
        {
            try
            {
                var fare = await _context.BusFares.FindAsync(id);
                if (fare == null)
                    return NotFound(new { success = false, message = "Fare not found" });

                return Ok(new { success = true, data = fare, message = "Fare retrieved successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving fare");
                return BadRequest(new { success = false, message = "Error retrieving fare", error = ex.Message });
            }
        }

        // POST: api/busfare
        [HttpPost]
        public async Task<ActionResult<BusFare>> AddFare([FromBody] BusFare fare)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new { success = false, message = "Invalid data", errors = ModelState });

                if (fare.FarePerKm <= 0)
                    return BadRequest(new { success = false, message = "Fare per km must be greater than 0" });

                fare.CreatedAt = DateTime.Now;
                _context.BusFares.Add(fare);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetFareById), new { id = fare.FareId },
                    new { success = true, data = fare, message = "Fare added successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding fare");
                return BadRequest(new { success = false, message = "Error adding fare", error = ex.Message });
            }
        }

        // PUT: api/busfare/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateFare(int id, [FromBody] BusFare fare)
        {
            try
            {
                if (id != fare.FareId)
                    return BadRequest(new { success = false, message = "ID mismatch" });

                var existingFare = await _context.BusFares.FindAsync(id);
                if (existingFare == null)
                    return NotFound(new { success = false, message = "Fare not found" });

                if (fare.FarePerKm <= 0)
                    return BadRequest(new { success = false, message = "Fare per km must be greater than 0" });

                existingFare.FarePerKm = fare.FarePerKm;
                existingFare.Description = fare.Description;
                existingFare.UpdatedAt = DateTime.Now;

                _context.BusFares.Update(existingFare);
                await _context.SaveChangesAsync();

                return Ok(new { success = true, data = existingFare, message = "Fare updated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating fare");
                return BadRequest(new { success = false, message = "Error updating fare", error = ex.Message });
            }
        }

        // DELETE: api/busfare/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFare(int id)
        {
            try
            {
                var fare = await _context.BusFares.FindAsync(id);
                if (fare == null)
                    return NotFound(new { success = false, message = "Fare not found" });

                _context.BusFares.Remove(fare);
                await _context.SaveChangesAsync();

                return Ok(new { success = true, message = "Fare deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting fare");
                return BadRequest(new { success = false, message = "Error deleting fare", error = ex.Message });
            }
        }

        // GET: api/busfare/calculate - Calculate fare based on distance
        [HttpGet("calculate/{distance}")]
        public async Task<IActionResult> CalculateFare(decimal distance)
        {
            try
            {
                if (distance <= 0)
                    return BadRequest(new { success = false, message = "Distance must be greater than 0" });

                var baseFare = await _context.BusFares.FirstOrDefaultAsync();
                if (baseFare == null)
                    return NotFound(new { success = false, message = "No fare rate configured" });

                decimal totalFare = baseFare.FarePerKm * distance;
                return Ok(new { success = true, data = new { distance, farePerKm = baseFare.FarePerKm, totalFare }, message = "Fare calculated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating fare");
                return BadRequest(new { success = false, message = "Error calculating fare", error = ex.Message });
            }
        }
    }
}
