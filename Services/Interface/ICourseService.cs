using OnlineCourses.Models.DTOs;

namespace OnlineCourses.Services.Interface
{
    public interface ICourseService
    {
        Task<ApiResponse<IEnumerable<CourseDto>>> GetAllAsync();
        Task<ApiResponse<IEnumerable<ScheduleDto>>> GetSchedulesByIdAsync(int courseId);
    }
}
