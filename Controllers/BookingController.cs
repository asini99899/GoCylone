using GoCylone.Data;
using GoCylone.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GoCylone.Controllers;

[Route("Booking")]
public class BookingController : Controller
{
    private readonly GoCyloneDbContext _context;
    private readonly ILogger<BookingController> _logger;

    public BookingController(GoCyloneDbContext context, ILogger<BookingController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // GET: Booking/SelectSeats/:scheduleId
    [HttpGet("SelectSeats/{scheduleId}")]
    public async Task<IActionResult> SelectSeats(int scheduleId)
    {
        try
        {
            var schedule = await _context.Schedules
                .Include(s => s.Bus)
                .Include(s => s.Route)
                .FirstOrDefaultAsync(s => s.ScheduleId == scheduleId);

            if (schedule == null)
                return NotFound("Schedule not found");

            // Clean up expired holds (older than 20 minutes)
            var expiredHolds = await _context.BookingSeats
                .Where(bs => bs.ScheduleId == scheduleId &&
                             bs.Status == "Processing" &&
                             bs.HoldExpiryTime.HasValue &&
                             bs.HoldExpiryTime < DateTime.Now)
                .ToListAsync();

            if (expiredHolds.Any())
            {
                _context.BookingSeats.RemoveRange(expiredHolds);
                await _context.SaveChangesAsync();
            }

            // Get booked seats (permanently booked only)
            var bookedSeats = await _context.BookingSeats
                .Where(bs => bs.ScheduleId == scheduleId && bs.Status == "Booked")
                .Select(bs => bs.SeatNumber)
                .ToListAsync();

            // Get processing seats (temporarily held)
            var processingSeats = await _context.BookingSeats
                .Where(bs => bs.ScheduleId == scheduleId &&
                             bs.Status == "Processing" &&
                             bs.HoldExpiryTime > DateTime.Now)
                .Select(bs => bs.SeatNumber)
                .ToListAsync();

            var viewModel = new SelectSeatsViewModel
            {
                ScheduleId = scheduleId,
                BusNumberPlate = schedule.Bus!.NumberPlate,
                SeatStructure = schedule.Bus.SeatStructure,
                TotalSeats = schedule.Bus.NumberOfSeats,
                FromLocation = schedule.Route!.FromLocation,
                ToLocation = schedule.Route.ToLocation,
                DepartureTime = schedule.DepartureTime,
                Distance = schedule.Route.Distance,
                EstimatedTime = schedule.Route.EstimatedTime,
                BookedSeats = bookedSeats,
                ProcessingSeats = processingSeats
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading seat selection page");
            return BadRequest("Error loading seat selection page");
        }
    }

    // POST: Booking/ValidateSeats
    [HttpPost("ValidateSeats")]
    public async Task<IActionResult> ValidateSeats([FromBody] ValidateSeatsRequest request)
    {
        try
        {
            if (request == null || !request.SelectedSeats.Any())
                return BadRequest(new { success = false, message = "Please select at least one seat" });

            var schedule = await _context.Schedules
                .Include(s => s.Bus)
                .Include(s => s.Route)
                .FirstOrDefaultAsync(s => s.ScheduleId == request.ScheduleId);

            if (schedule == null)
                return BadRequest(new { success = false, message = "Schedule not found" });

            // Clean up expired holds
            var expiredHolds = await _context.BookingSeats
                .Where(bs => bs.ScheduleId == request.ScheduleId &&
                             bs.Status == "Processing" &&
                             bs.HoldExpiryTime.HasValue &&
                             bs.HoldExpiryTime < DateTime.Now)
                .ToListAsync();

            if (expiredHolds.Any())
            {
                _context.BookingSeats.RemoveRange(expiredHolds);
                await _context.SaveChangesAsync();
            }

            // Get booked seats (permanent)
            var bookedSeats = await _context.BookingSeats
                .Where(bs => bs.ScheduleId == request.ScheduleId && bs.Status == "Booked")
                .Select(bs => bs.SeatNumber)
                .ToListAsync();

            // Get processing seats (temp hold)
            var processingSeats = await _context.BookingSeats
                .Where(bs => bs.ScheduleId == request.ScheduleId &&
                             bs.Status == "Processing" &&
                             bs.HoldExpiryTime > DateTime.Now)
                .Select(bs => bs.SeatNumber)
                .ToListAsync();

            // Combine both for validation
            var unavailableSeats = bookedSeats.Union(processingSeats).ToList();

            var selectedSeatsUnavailable = request.SelectedSeats.Where(s => unavailableSeats.Contains(s)).ToList();
            if (selectedSeatsUnavailable.Any())
            {
                var bookedSeatsList = selectedSeatsUnavailable.Where(s => bookedSeats.Contains(s)).ToList();
                var processingSeatsList = selectedSeatsUnavailable.Where(s => processingSeats.Contains(s)).ToList();

                string message = "";
                if (bookedSeatsList.Any())
                    message += $"Seats {string.Join(", ", bookedSeatsList)} are already booked. ";
                if (processingSeatsList.Any())
                    message += $"Seats {string.Join(", ", processingSeatsList)} are being held by another user (expires in 20 minutes).";

                return BadRequest(new { success = false, message = message.Trim() });
            }

            return Ok(new { success = true, message = "Seats are available" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating seats");
            return BadRequest(new { success = false, message = "Error validating seats", error = ex.Message });
        }
    }

    // GET: Booking/ConfirmBooking/:scheduleId
    [HttpGet("ConfirmBooking/{scheduleId}")]
    public async Task<IActionResult> ConfirmBooking(int scheduleId, [FromQuery] string seats = "", [FromQuery] string pickup = "", [FromQuery] string drop = "")
    {
        try
        {
            _logger.LogInformation($"ConfirmBooking called - ScheduleId: {scheduleId}, Seats: '{seats}', Pickup: {pickup}, Drop: {drop}");

            // Parse seats - allow empty string for no seats selected
            List<int> seatList = new List<int>();
            if (!string.IsNullOrWhiteSpace(seats))
            {
                try
                {
                    seatList = seats.Split(',')
                        .Where(s => !string.IsNullOrWhiteSpace(s))
                        .Select(s => int.Parse(s.Trim()))
                        .OrderBy(s => s)
                        .ToList();
                    _logger.LogInformation($"Parsed seats: {string.Join(", ", seatList)}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error parsing seats: {seats}");
                    seatList = new List<int>();
                }
            }

            var schedule = await _context.Schedules
                .Include(s => s.Bus)
                .Include(s => s.Route)
                .FirstOrDefaultAsync(s => s.ScheduleId == scheduleId);

            if (schedule == null)
                return NotFound("Schedule not found");

            // Get fare per km
            var farePerKm = await _context.BusFares.FirstOrDefaultAsync();
            decimal farePerKmValue = farePerKm?.FarePerKm ?? 10; // Default 10 per km

            // Calculate total fare: (Distance × FarePerKm) + ServiceCharge(20)
            decimal baseFare = schedule.Route!.Distance * farePerKmValue;
            decimal serviceCharge = 20;
            decimal totalFare = baseFare + serviceCharge;

            var viewModel = new ConfirmBookingViewModel
            {
                ScheduleId = scheduleId,
                BusNumberPlate = schedule.Bus!.NumberPlate,
                FromLocation = schedule.Route!.FromLocation,
                ToLocation = schedule.Route.ToLocation,
                DepartureTime = schedule.DepartureTime,
                SelectedSeats = seatList,
                PickupLocation = pickup ?? schedule.Route.FromLocation,
                DropLocation = drop ?? schedule.Route.ToLocation,
                Distance = schedule.Route.Distance,
                FarePerKm = farePerKmValue,
                BaseFare = baseFare,
                ServiceCharge = serviceCharge,
                TotalFare = totalFare
            };

            _logger.LogInformation($"ConfirmBooking ViewModel created with {seatList.Count} seats");

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading confirmation page");
            return BadRequest("Error loading confirmation page");
        }
    }

    // POST: Booking/ProcessPayment
    [HttpPost("ProcessPayment")]
    public async Task<IActionResult> ProcessPayment([FromBody] PaymentRequest request)
    {
        try
        {
            if (request == null)
                return BadRequest(new { success = false, message = "Invalid booking data" });

            // Validate card details (basic validation)
            if (string.IsNullOrWhiteSpace(request.CardHolderName) ||
                string.IsNullOrWhiteSpace(request.CardNumber) ||
                string.IsNullOrWhiteSpace(request.ExpiryDate) ||
                string.IsNullOrWhiteSpace(request.CVV))
                return BadRequest(new { success = false, message = "Please fill all card details" });

            // Clean up expired holds first
            var expiredHolds = await _context.BookingSeats
                .Where(bs => bs.ScheduleId == request.ScheduleId &&
                             bs.Status == "Processing" &&
                             bs.HoldExpiryTime.HasValue &&
                             bs.HoldExpiryTime < DateTime.Now)
                .ToListAsync();

            if (expiredHolds.Any())
            {
                _context.BookingSeats.RemoveRange(expiredHolds);
                await _context.SaveChangesAsync();
            }

            // If seats were selected, verify they are still available
            if (request.SelectedSeats != null && request.SelectedSeats.Any())
            {
                var conflictingSeats = await _context.BookingSeats
                    .Where(bs => bs.ScheduleId == request.ScheduleId &&
                                 bs.Status == "Booked" &&
                                 request.SelectedSeats.Contains(bs.SeatNumber))
                    .Select(bs => bs.SeatNumber)
                    .ToListAsync();

                if (conflictingSeats.Any())
                    return BadRequest(new { success = false, message = $"Seats {string.Join(", ", conflictingSeats)} were booked by another user. Please refresh and select again." });
            }

            // Create booking
            var booking = new Booking
            {
                UserId = 1, // TODO: Get from logged-in user
                ScheduleId = request.ScheduleId,
                NumberOfSeats = request.SelectedSeats != null ? request.SelectedSeats.Count : 0,
                TotalFare = request.TotalFare,
                Status = "Confirmed",
                PickupLocation = request.PickupLocation,
                DropLocation = request.DropLocation,
                BookingDate = DateTime.Now,
                BookedDate = DateTime.Now,
                // Generate reference number format: BK-YYYYMMDD-XXXXX
                ReferenceNumber = $"BK-{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 5).ToUpper()}",
                // TODO: Get user details from logged-in user
                UserName = request.CardHolderName,
                UserEmail = "user@example.com",
                UserPhone = "+1-XXX-XXX-XXXX",
                SeatNumbers = request.SelectedSeats != null && request.SelectedSeats.Any()
                    ? string.Join(",", request.SelectedSeats.OrderBy(s => s))
                    : string.Empty
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            // Get schedule and route information for response
            var schedule = await _context.Schedules
                .Include(s => s.Bus)
                .Include(s => s.Route)
                .FirstOrDefaultAsync(s => s.ScheduleId == request.ScheduleId);

            if (schedule == null)
                return BadRequest(new { success = false, message = "Schedule not found" });

            // Only process seats if any were selected
            if (request.SelectedSeats != null && request.SelectedSeats.Any())
            {
                // Remove any "Processing" holds for this user and set selected seats as "Booked"
                var processingSeats = await _context.BookingSeats
                    .Where(bs => bs.ScheduleId == request.ScheduleId && bs.Status == "Processing")
                    .ToListAsync();

                foreach (var ps in processingSeats)
                {
                    if (request.SelectedSeats.Contains(ps.SeatNumber))
                    {
                        // Update this to booked
                        ps.Status = "Booked";
                        ps.BookingId = booking.BookingId;
                        ps.BookedDate = DateTime.Now;
                    }
                    else
                    {
                        // Remove other processing holds
                        _context.BookingSeats.Remove(ps);
                    }
                }

                // Create new booking seat records for any seats that don't have processing records
                foreach (var seatNumber in request.SelectedSeats)
                {
                    var existingBookingSeat = await _context.BookingSeats
                        .FirstOrDefaultAsync(bs => bs.ScheduleId == request.ScheduleId &&
                                                  bs.SeatNumber == seatNumber &&
                                                  bs.Status == "Processing");

                    if (existingBookingSeat == null)
                    {
                        var bookingSeat = new BookingSeat
                        {
                            BookingId = booking.BookingId,
                            SeatNumber = seatNumber,
                            ScheduleId = request.ScheduleId,
                            Status = "Booked",
                            BookedDate = DateTime.Now
                        };
                        _context.BookingSeats.Add(bookingSeat);
                    }
                }
            }

            await _context.SaveChangesAsync();

            // Create payment record
            var payment = new PaymentInfo
            {
                BookingId = booking.BookingId,
                CardHolderName = request.CardHolderName,
                CardNumber = request.CardNumber.Substring(request.CardNumber.Length - 4).PadLeft(request.CardNumber.Length, '*'),
                ExpiryDate = request.ExpiryDate,
                CVV = "***", // Never store actual CVV
                Amount = request.TotalFare,
                PaymentStatus = "Completed",
                TransactionId = Guid.NewGuid().ToString(),
                PaymentDate = DateTime.Now
            };

            _context.PaymentInfos.Add(payment);
            await _context.SaveChangesAsync();

            // Build complete booking response
            var bookingResponse = new BookingResponseModel
            {
                Success = true,
                Message = "Booking confirmed successfully!",
                ReferenceNumber = booking.ReferenceNumber,
                BookingId = booking.BookingId,
                BookedDate = booking.BookedDate,

                // Bus Information
                BusNumberPlate = schedule.Bus!.NumberPlate,

                // User Information
                UserName = booking.UserName,
                UserEmail = booking.UserEmail,
                UserPhone = booking.UserPhone,

                // Route Information
                FromLocation = schedule.Route!.FromLocation,
                ToLocation = schedule.Route.ToLocation,
                PickupLocation = request.PickupLocation,
                DropLocation = request.DropLocation,

                // Departure Information
                DepartureDateTime = schedule.ScheduledDate.Date.Add(schedule.DepartureTime),
                DepartureTime = schedule.DepartureTime,

                // Seat Information
                SeatNumbers = request.SelectedSeats ?? new List<int>(),
                NumberOfSeats = request.SelectedSeats != null ? request.SelectedSeats.Count : 0,

                // Fare Information
                Distance = schedule.Route.Distance,
                FarePerKm = 50, // TODO: Get from fare table
                BaseFare = request.TotalFare - 20,
                ServiceCharge = 20,
                TotalFare = request.TotalFare,

                // Payment Information
                MaskedCardNumber = payment.CardNumber
            };

            return Ok(bookingResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing payment");
            return BadRequest(new { success = false, message = "Error processing payment", error = ex.Message });
        }
    }

    // POST: Booking/HoldSeats - Create a temporary hold on seats
    [HttpPost("HoldSeats")]
    public async Task<IActionResult> HoldSeats([FromBody] HoldSeatsRequest request)
    {
        try
        {
            if (request == null || !request.SelectedSeats.Any())
                return BadRequest(new { success = false, message = "No seats to hold" });

            // Clean up expired holds first
            var expiredHolds = await _context.BookingSeats
                .Where(bs => bs.ScheduleId == request.ScheduleId &&
                             bs.Status == "Processing" &&
                             bs.HoldExpiryTime.HasValue &&
                             bs.HoldExpiryTime < DateTime.Now)
                .ToListAsync();

            if (expiredHolds.Any())
            {
                _context.BookingSeats.RemoveRange(expiredHolds);
                await _context.SaveChangesAsync();
            }

            // Get permanently booked seats
            var bookedSeats = await _context.BookingSeats
                .Where(bs => bs.ScheduleId == request.ScheduleId && bs.Status == "Booked")
                .Select(bs => bs.SeatNumber)
                .ToListAsync();

            // Check if any selected seats are already booked
            var conflictingSeats = request.SelectedSeats.Where(s => bookedSeats.Contains(s)).ToList();
            if (conflictingSeats.Any())
                return BadRequest(new { success = false, message = $"Seats {string.Join(", ", conflictingSeats)} are already booked" });

            // Create processing holds for selected seats (20 minute hold)
            var holdExpiryTime = DateTime.Now.AddMinutes(20);

            foreach (var seatNumber in request.SelectedSeats)
            {
                // Check if seat already has an active hold
                var existingHold = await _context.BookingSeats
                    .FirstOrDefaultAsync(bs => bs.ScheduleId == request.ScheduleId &&
                                              bs.SeatNumber == seatNumber &&
                                              bs.Status == "Processing" &&
                                              bs.HoldExpiryTime > DateTime.Now);

                if (existingHold == null)
                {
                    var hold = new BookingSeat
                    {
                        ScheduleId = request.ScheduleId,
                        SeatNumber = seatNumber,
                        Status = "Processing",
                        ProcessingStartTime = DateTime.Now,
                        HoldExpiryTime = holdExpiryTime,
                        BookingId = 0 // Temporary placeholder
                    };
                    _context.BookingSeats.Add(hold);
                }
            }

            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Seats held for 20 minutes", expiryTime = holdExpiryTime });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error holding seats");
            return BadRequest(new { success = false, message = "Error holding seats", error = ex.Message });
        }
    }

    // POST: Booking/ReleaseHold - Release temporary holds (user cancels)
    [HttpPost("ReleaseHold")]
    public async Task<IActionResult> ReleaseHold([FromBody] ReleaseHoldRequest request)
    {
        try
        {
            var holds = await _context.BookingSeats
                .Where(bs => bs.ScheduleId == request.ScheduleId &&
                             bs.Status == "Processing" &&
                             request.SeatNumbers.Contains(bs.SeatNumber))
                .ToListAsync();

            _context.BookingSeats.RemoveRange(holds);
            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Seat hold released" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error releasing hold");
            return BadRequest(new { success = false, message = "Error releasing hold", error = ex.Message });
        }
    }

    // GET: Booking/Success/:bookingId
    [HttpGet("Success/{bookingId}")]
    public async Task<IActionResult> Success(int bookingId)
    {
        try
        {
            var booking = await _context.Bookings
                .Include(b => b.Schedule)
                .Include(b => b.Schedule!.Bus)
                .Include(b => b.Schedule!.Route)
                .Include(b => b.BookingSeats)
                .FirstOrDefaultAsync(b => b.BookingId == bookingId);

            if (booking == null)
                return NotFound("Booking not found");

            return View(booking);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading booking success page");
            return BadRequest("Error loading booking success page");
        }
    }
}

// View Models
public class SelectSeatsViewModel
{
    public int ScheduleId { get; set; }
    public string BusNumberPlate { get; set; } = string.Empty;
    public string SeatStructure { get; set; } = string.Empty; // "2*2", "2*3"
    public int TotalSeats { get; set; }
    public string FromLocation { get; set; } = string.Empty;
    public string ToLocation { get; set; } = string.Empty;
    public TimeSpan DepartureTime { get; set; }
    public decimal Distance { get; set; }
    public string EstimatedTime { get; set; } = string.Empty;
    public List<int> BookedSeats { get; set; } = new List<int>();
    public List<int> ProcessingSeats { get; set; } = new List<int>();
}

public class ConfirmBookingViewModel
{
    public int ScheduleId { get; set; }
    public string BusNumberPlate { get; set; } = string.Empty;
    public string FromLocation { get; set; } = string.Empty;
    public string ToLocation { get; set; } = string.Empty;
    public TimeSpan DepartureTime { get; set; }
    public List<int> SelectedSeats { get; set; } = new List<int>();
    public string PickupLocation { get; set; } = string.Empty;
    public string DropLocation { get; set; } = string.Empty;
    public decimal Distance { get; set; }
    public decimal FarePerKm { get; set; }
    public decimal BaseFare { get; set; }
    public decimal ServiceCharge { get; set; } = 20;
    public decimal TotalFare { get; set; }
}

public class ValidateSeatsRequest
{
    public int ScheduleId { get; set; }
    public List<int> SelectedSeats { get; set; } = new List<int>();
}

public class PaymentRequest
{
    public int ScheduleId { get; set; }
    public List<int> SelectedSeats { get; set; } = new List<int>();
    public string PickupLocation { get; set; } = string.Empty;
    public string DropLocation { get; set; } = string.Empty;
    public decimal TotalFare { get; set; }
    public string CardHolderName { get; set; } = string.Empty;
    public string CardNumber { get; set; } = string.Empty;
    public string ExpiryDate { get; set; } = string.Empty;
    public string CVV { get; set; } = string.Empty;
}

public class HoldSeatsRequest
{
    public int ScheduleId { get; set; }
    public List<int> SelectedSeats { get; set; } = new List<int>();
}

public class ReleaseHoldRequest
{
    public int ScheduleId { get; set; }
    public List<int> SeatNumbers { get; set; } = new List<int>();
}

public class BookingResponseModel
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string ReferenceNumber { get; set; } = string.Empty;
    public int BookingId { get; set; }
    public DateTime BookedDate { get; set; }

    // Bus Information
    public string BusNumberPlate { get; set; } = string.Empty;

    // User Information
    public string UserName { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
    public string UserPhone { get; set; } = string.Empty;

    // Route Information
    public string FromLocation { get; set; } = string.Empty;
    public string ToLocation { get; set; } = string.Empty;
    public string PickupLocation { get; set; } = string.Empty;
    public string DropLocation { get; set; } = string.Empty;

    // Departure Information
    public DateTime DepartureDateTime { get; set; }
    public TimeSpan DepartureTime { get; set; }

    // Seat Information
    public List<int> SeatNumbers { get; set; } = new List<int>();
    public int NumberOfSeats { get; set; }

    // Fare Information
    public decimal Distance { get; set; }
    public decimal FarePerKm { get; set; }
    public decimal BaseFare { get; set; }
    public decimal ServiceCharge { get; set; } = 20;
    public decimal TotalFare { get; set; }

    // Payment Information
    public string MaskedCardNumber { get; set; } = string.Empty;
}
