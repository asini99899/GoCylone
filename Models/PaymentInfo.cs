namespace GoCylone.Models
{
    public class PaymentInfo
    {
        public int PaymentId { get; set; }
        public int BookingId { get; set; }
        public string CardHolderName { get; set; } = string.Empty;
        public string CardNumber { get; set; } = string.Empty; // Should be encrypted in production
        public string ExpiryDate { get; set; } = string.Empty; // MM/YY format
        public string CVV { get; set; } = string.Empty; // Should be encrypted in production
        public decimal Amount { get; set; }
        public string PaymentStatus { get; set; } = "Pending"; // Pending, Completed, Failed
        public DateTime PaymentDate { get; set; } = DateTime.Now;
        public string TransactionId { get; set; } = string.Empty;
        public DateTime? UpdatedAt { get; set; }

        // Navigation property
        public Booking? Booking { get; set; }
    }
}
