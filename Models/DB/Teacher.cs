﻿namespace OnlineCourses.Models.DB
{
    public class Teacher
    {
        public int Id { get; set; }
        public int UniversityId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
    }
}
