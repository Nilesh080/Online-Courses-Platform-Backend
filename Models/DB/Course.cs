namespace OnlineCourses.Models.DB
{
    public class Course
    {
        public int Id { get; set; }
        public int UniversityId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int TotalHours { get; set; }
    }
}
