namespace GoCylone.Models
{
    public class Route
    {
        public int RouteId { get; set; }
        public string FromLocation { get; set; } = string.Empty;
        public string ToLocation { get; set; } = string.Empty;
        public decimal Distance { get; set; } // in kilometers
        public string EstimatedTime { get; set; } = string.Empty; // e.g., "2h 30m"
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }

        // Navigation property
        public ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
    }
}
