namespace OnlineCourses.Models.DTOs
{
    public class EnrollmentDto
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int ScheduleId { get; set; }
        public string CurrentStatus { get; set; } = string.Empty;
    }
}
