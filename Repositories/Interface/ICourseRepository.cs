using OnlineCourses.Models.DB;
using OnlineCourses.Models.DTOs;

namespace OnlineCourses.Repositories.Interface
{
    public interface ICourseRepository
    {
        Task<IEnumerable<CourseDto>> GetAllAsync();
        Task<Course?> GetByIdAsync(int courseId);
        Task<IEnumerable<ScheduleDto>> GetSchedulesByIdAsync(int courseId);
        Task<Schedule?> GetScheduleByIdAsync(int scheduleId);
        Task<List<ScheduleDto>> GetSchedulesByIdsAsync(List<int> scheduleIds);
    }
}
