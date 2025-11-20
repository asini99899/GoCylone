namespace GoCylone.Models
{
    public class Bus
    {
        public int BusId { get; set; }
        public string NumberPlate { get; set; } = string.Empty;
        public int NumberOfSeats { get; set; }
        public string SeatStructure { get; set; } = string.Empty; // "2*2" or "2*3"
        public string ConductorNumber { get; set; } = string.Empty;
        public string Condition { get; set; } = string.Empty; // "AC" or "Non-AC"
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }

        // Navigation property
        public ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
    }
}
