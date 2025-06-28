using OnlineCourses.Helper;
using OnlineCourses.Models.DTOs;
using OnlineCourses.Repositories.Interface;
using OnlineCourses.Services.Interface;

namespace OnlineCourses.Services
{
    public class EnrollmentService : IEnrollmentService
    {
        private readonly IEnrollmentRepository _enrollmentRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly ICourseRepository _courseRepository;

        public EnrollmentService(
            IEnrollmentRepository enrollmentRepository,
            IStudentRepository studentRepository,
            ICourseRepository courseRepository)
        {
            _enrollmentRepository = enrollmentRepository;
            _studentRepository = studentRepository;
            _courseRepository = courseRepository;
        }

        public async Task<ApiResponse<EnrollmentResponseDto>> EnrollStudentAsync(EnrollmentRequestDto enrollmentRequest)
        {
            try
            {
                var student = await _studentRepository.GetStudentByIdAsync(enrollmentRequest.StudentId);
                if (student == null)
                {
                    return new ApiResponse<EnrollmentResponseDto>
                    {
                        Success = false,
                        Message = "Student not found",
                        Errors = new List<string> { $"Student with ID {enrollmentRequest.StudentId} does not exist" }
                    };
                }

                var schedule = await _courseRepository.GetScheduleByIdAsync(enrollmentRequest.ScheduleId);
                if (schedule == null)
                {
                    return new ApiResponse<EnrollmentResponseDto>
                    {
                        Success = false,
                        Message = "Schedule not found",
                        Errors = new List<string> { $"Schedule with ID {enrollmentRequest.ScheduleId} does not exist" }
                    };
                }

                if (!schedule.IsActive)
                {
                    return new ApiResponse<EnrollmentResponseDto>
                    {
                        Success = false,
                        Message = "Schedule is not active",
                        Errors = new List<string> { "Cannot enroll in an inactive schedule" }
                    };
                }

                var isAlreadyEnrolled = await _enrollmentRepository.IsStudentAlreadyEnrolledAsync(
                    enrollmentRequest.StudentId, enrollmentRequest.ScheduleId);

                if (isAlreadyEnrolled)
                {
                    return new ApiResponse<EnrollmentResponseDto>
                    {
                        Success = false,
                        Message = "Student already enrolled",
                        Errors = new List<string> { "Student is already enrolled in this schedule" }
                    };
                }

                List<int> ids = new List<int> { schedule.Id };

                var scheduleDtos = await _courseRepository.GetSchedulesByIdsAsync(ids);

                var current_schedule = scheduleDtos[0];

                var overlapValidationResult = await ValidateScheduleOverlapAsync(enrollmentRequest.StudentId, current_schedule);
                if (!overlapValidationResult.IsValid)
                {
                    return new ApiResponse<EnrollmentResponseDto>
                    {
                        Success = false,
                        Message = "Schedule conflict detected",
                        Errors = overlapValidationResult.Errors
                    };
                }

                var enrollmentId = await _enrollmentRepository.CreateEnrollmentAsync(enrollmentRequest);
                var enrollmentResponse = await _enrollmentRepository.GetEnrollmentByIdAsync(enrollmentId);

                return new ApiResponse<EnrollmentResponseDto>
                {
                    Success = true,
                    Message = "Student enrolled successfully",
                    Data = enrollmentResponse
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<EnrollmentResponseDto>
                {
                    Success = false,
                    Message = "An error occurred during enrollment",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        private async Task<ValidationResult> ValidateScheduleOverlapAsync(int studentId, ScheduleDto newSchedule)
        {
            var errors = new List<string>();

            try
            {
                var existingEnrollments = await _enrollmentRepository.GetActiveEnrollmentsByStudentIdAsync(studentId);

                if (!existingEnrollments.Any())
                {
                    return new ValidationResult { IsValid = true, Errors = new List<string>() };
                }

                var existingScheduleIds = existingEnrollments.Select(e => e.ScheduleId).ToList();
                var existingSchedules = await _courseRepository.GetSchedulesByIdsAsync(existingScheduleIds);

                var conflictingSchedules = existingSchedules
                    .Select(existingSchedule => new
                    {
                        Schedule = existingSchedule,
                        OverlapResult = CheckScheduleTimeOverlap(newSchedule, existingSchedule)
                    })
                    .Where(x => x.OverlapResult.HasOverlap)
                    .ToList();

                errors.AddRange(conflictingSchedules.Select(conflict =>
                    $"Schedule conflicts with existing enrollment in '{conflict.Schedule.CourseName}' - {conflict.OverlapResult.ConflictDetails}"));
            }
            catch (Exception ex)
            {
                errors.Add($"Unable to validate schedule overlaps at this time.{ex.Message}");
            }

            return new ValidationResult
            {
                IsValid = errors.Count == 0,
                Errors = errors
            };
        }

        private OverlapResult CheckScheduleTimeOverlap(ScheduleDto newSchedule, ScheduleDto existingSchedule)
        {
            var conflicts = newSchedule.TimeSlots
                .SelectMany(newTimeSlot => existingSchedule.TimeSlots
                    .Where(existingTimeSlot =>
                        newTimeSlot.DayOfWeek == existingTimeSlot.DayOfWeek &&
                        DoesTimeOverlap(newTimeSlot.StartTime, newTimeSlot.EndTime,
                                      existingTimeSlot.StartTime, existingTimeSlot.EndTime))
                    .Select(existingTimeSlot =>
                        $"{newTimeSlot.DayOfWeek}: {newTimeSlot.StartTime:hh\\:mm}-{newTimeSlot.EndTime:hh\\:mm} overlaps with {existingTimeSlot.StartTime:hh\\:mm}-{existingTimeSlot.EndTime:hh\\:mm}"))
                .ToList();

            return new OverlapResult
            {
                HasOverlap = conflicts.Any(),
                ConflictDetails = string.Join("; ", conflicts)
            };
        }

        private bool DoesTimeOverlap(TimeSpan start1, TimeSpan end1, TimeSpan start2, TimeSpan end2)
        {
            return start1 < end2 && start2 < end1;
        }
    }
}