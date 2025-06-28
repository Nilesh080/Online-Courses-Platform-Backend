using OnlineCourses.Models.DTOs;

namespace OnlineCourses.Services.Interface
{
    public interface ISchedulePlannerService
    {
        Task<ApiResponse<SchedulePlannerResponseDto>> GetOptimalScheduleAsync(SchedulePlannerRequestDto request);
    }
}
