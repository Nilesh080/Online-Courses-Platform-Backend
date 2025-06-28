namespace OnlineCourses.Models.DB
{
    public class Student
    {
        public int Id { get; set; }
        public int UniversityId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime DOB { get; set; }
    }
}
