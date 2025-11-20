namespace GoCylone.Models
{
    public class BusFare
    {
        public int FareId { get; set; }
        public decimal FarePerKm { get; set; }
        public string Description { get; set; } = string.Empty; // e.g., "Base fare per km"
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
    }
}
