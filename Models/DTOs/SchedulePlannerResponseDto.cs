namespace OnlineCourses.Models.DTOs
{
    public class SchedulePlannerResponseDto
    {
        public List<int> SelectedCourseIds { get; set; } = new();
        public string Message { get; set; } = string.Empty;
    }
}
