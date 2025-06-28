using System.ComponentModel.DataAnnotations;

namespace OnlineCourses.Models.DTOs
{
    public class LoginDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
