using OnlineCourses.Models.DTOs;

namespace OnlineCourses.Repositories.Interface
{
    public interface IEnrollmentRepository
    {
        Task<int> CreateEnrollmentAsync(EnrollmentRequestDto enrollmentRequest);
        Task<EnrollmentResponseDto?> GetEnrollmentByIdAsync(int enrollmentId);
        Task<bool> IsStudentAlreadyEnrolledAsync(int studentId, int scheduleId);
        Task<List<EnrollmentDto>> GetActiveEnrollmentsByStudentIdAsync(int studentId);
    }
}
