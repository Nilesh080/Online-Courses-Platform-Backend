namespace OnlineCourses.Models.DTOs
{
    public class SchedulePlannerRequestDto
    {
        public List<int> PreferredCourseIds { get; set; } = new();
        public List<string> AvailableDays { get; set; } = new();
    }
}
