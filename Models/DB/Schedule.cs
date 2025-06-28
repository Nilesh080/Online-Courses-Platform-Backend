namespace OnlineCourses.Models.DB
{
    public class Schedule
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public int TeacherId { get; set; }
        public bool IsActive { get; set; }
    }
}
