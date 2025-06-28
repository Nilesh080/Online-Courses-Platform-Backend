using System.ComponentModel.DataAnnotations;

namespace OnlineCourses.Models.DTOs
{
    public class EnrollmentRequestDto
    {
        [Required]
        public int StudentId { get; set; }

        [Required]
        public int ScheduleId { get; set; }
    }
}
