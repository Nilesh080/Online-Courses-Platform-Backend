using OnlineCourses.Models.DB;

namespace OnlineCourses.Repositories.Interface
{
    public interface ITeacherRepository
    {
        Task<Teacher?> GetTeacherByIdAsync(int teacherId);
        Task<Teacher?> GetTeacherByEmailAsync(string email);
    }
}
