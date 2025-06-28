using OnlineCourses.Models.DTOs;

namespace OnlineCourses.Services.Interface
{
    public interface IEnrollmentService
    {
        Task<ApiResponse<EnrollmentResponseDto>> EnrollStudentAsync(EnrollmentRequestDto enrollmentRequest);
    }
}
