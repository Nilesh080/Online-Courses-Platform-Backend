namespace OnlineCourses.Models.DB
{
    public class ScheduleTimeSlot
    {
        public int Id { get; set; }
        public int ScheduleId { get; set; }
        public string DayOfWeek { get; set; } = string.Empty;
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
}
