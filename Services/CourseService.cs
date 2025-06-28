using OnlineCourses.Models.DTOs;
using OnlineCourses.Repositories.Interface;
using OnlineCourses.Services.Interface;

namespace OnlineCourses.Services
{
    public class CourseService : ICourseService
    {
        private readonly ICourseRepository _courseRepository;

        public CourseService(ICourseRepository courseRepository)
        {
            _courseRepository = courseRepository;
        }

        public async Task<ApiResponse<IEnumerable<CourseDto>>> GetAllAsync()
        {
            try
            {
                var courses = await _courseRepository.GetAllAsync();

                return new ApiResponse<IEnumerable<CourseDto>>
                {
                    Success = true,
                    Message = "Courses retrieved successfully",
                    Data = courses
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<IEnumerable<CourseDto>>
                {
                    Success = false,
                    Message = "An error occurred while retrieving courses",
                    Errors = new List<string> { ex.Message }
                };
            }
        }
        public async Task<ApiResponse<IEnumerable<ScheduleDto>>> GetSchedulesByIdAsync(int courseId)
        {
            try
            {
                // Validate if course exists
                var course = await _courseRepository.GetByIdAsync(courseId);
                if (course == null)
                {
                    return new ApiResponse<IEnumerable<ScheduleDto>>
                    {
                        Success = false,
                        Message = "Course not found",
                        Errors = new List<string> { $"Course with ID {courseId} does not exist" }
                    };
                }

                var schedules = await _courseRepository.GetSchedulesByIdAsync(courseId);

                return new ApiResponse<IEnumerable<ScheduleDto>>
                {
                    Success = true,
                    Message = "Schedules retrieved successfully",
                    Data = schedules
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<IEnumerable<ScheduleDto>>
                {
                    Success = false,
                    Message = "An error occurred while retrieving schedules",
                    Errors = new List<string> { ex.Message }
                };
            }
        }
    }
}
