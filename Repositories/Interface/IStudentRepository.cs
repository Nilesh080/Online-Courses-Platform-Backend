using OnlineCourses.Models.DB;

namespace OnlineCourses.Repositories.Interface
{
    public interface IStudentRepository
    {
        Task<Student?> GetStudentByIdAsync(int studentId);
        Task<Student?> GetStudentByEmailAsync(string email);
    }
}
