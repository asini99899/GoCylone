namespace GoCylone.Models
{
    public class BookingSeat
    {
        public int BookingSeatId { get; set; }
        public int BookingId { get; set; }
        public int SeatNumber { get; set; }
        public int ScheduleId { get; set; }
        public string Status { get; set; } = "Booked"; // "Available", "Processing" (temp hold), "Booked" (confirmed)
        public DateTime BookedDate { get; set; } = DateTime.Now;
        public DateTime? ProcessingStartTime { get; set; } // When user selected the seat
        public DateTime? HoldExpiryTime { get; set; } // When the hold expires (20 min from processing start)
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public Booking? Booking { get; set; }
        public Schedule? Schedule { get; set; }
    }
}
