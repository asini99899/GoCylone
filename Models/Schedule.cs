namespace GoCylone.Models
{
    public class Schedule
    {
        public int ScheduleId { get; set; }
        public int BusId { get; set; }
        public int RouteId { get; set; }
        public DateTime ScheduledDate { get; set; }
        public TimeSpan DepartureTime { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public Bus? Bus { get; set; }
        public Route? Route { get; set; }
    }
}
