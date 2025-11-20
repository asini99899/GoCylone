namespace GoCylone.Models
{
    public class Booking
    {
        public int BookingId { get; set; }
        public int UserId { get; set; }
        public int ScheduleId { get; set; }
        public int NumberOfSeats { get; set; }
        public decimal TotalFare { get; set; }
        public string Status { get; set; } = "Pending"; // Pending, Confirmed, Cancelled
        public string PickupLocation { get; set; } = string.Empty; // Boarding place
        public string DropLocation { get; set; } = string.Empty; // Drop place
        public DateTime BookingDate { get; set; } = DateTime.Now;
        public DateTime? PaymentDate { get; set; }
        public DateTime? UpdatedAt { get; set; }
        
        // New fields for complete booking information
        public string ReferenceNumber { get; set; } = string.Empty; // Unique reference number (e.g., BK-20251119-001)
        public DateTime BookedDate { get; set; } = DateTime.Now; // When booking was confirmed
        public string? BusNumberPlate { get; set; } // Bus number plate for quick reference
        public string? UserName { get; set; } // User's full name for booking
        public string? UserEmail { get; set; } // User's email for confirmation
        public string? UserPhone { get; set; } // User's phone number for communication
        public string? SeatNumbers { get; set; } // Comma-separated seat numbers (e.g., "2,3,4")

        // Navigation properties
        public User? User { get; set; }
        public Schedule? Schedule { get; set; }
        public ICollection<BookingSeat> BookingSeats { get; set; } = new List<BookingSeat>();
        public PaymentInfo? Payment { get; set; }
    }
}
