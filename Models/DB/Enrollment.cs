namespace OnlineCourses.Models.DB
{
    public class Enrollment
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int ScheduleId { get; set; }
        public string CurrentStatus { get; set; } = "Active";
    }
}
