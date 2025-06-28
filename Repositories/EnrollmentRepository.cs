using OnlineCourses.Models.DTOs;
using OnlineCourses.Repositories.Interface;

namespace OnlineCourses.Repositories
{
    public class EnrollmentRepository : BaseRepository, IEnrollmentRepository
    {
        public EnrollmentRepository(IConfiguration configuration)
            : base(configuration.GetConnectionString("DefaultConnection"))
        {
        }

        public async Task<int> CreateEnrollmentAsync(EnrollmentRequestDto enrollmentRequest)
        {
            const string query = @"
                INSERT INTO Enrollments (student_id, schedule_id, current_status)
                VALUES (@StudentId, @ScheduleId, 'Active');
                SELECT CAST(SCOPE_IDENTITY() as int);";

            return await QuerySingleAsync<int>(query, enrollmentRequest);
        }

        public async Task<EnrollmentResponseDto?> GetEnrollmentByIdAsync(int enrollmentId)
        {
            const string query = @"
                SELECT 
                    e.id,
                    e.student_id as StudentId,
                    s.name as StudentName,
                    e.schedule_id as ScheduleId,
                    c.name as CourseName,
                    t.name as TeacherName,
                    e.current_status as CurrentStatus,
                    GETDATE() as EnrollmentDate
                FROM Enrollments e
                INNER JOIN Students s ON e.student_id = s.id
                INNER JOIN Schedules sc ON e.schedule_id = sc.id
                INNER JOIN Courses c ON sc.course_id = c.id
                INNER JOIN Teachers t ON sc.teacher_id = t.id
                WHERE e.id = @EnrollmentId";

            return await QueryFirstOrDefaultAsync<EnrollmentResponseDto>(query, new { EnrollmentId = enrollmentId });
        }

        public async Task<bool> IsStudentAlreadyEnrolledAsync(int studentId, int scheduleId)
        {
            const string query = @"
                SELECT COUNT(1) 
                FROM Enrollments 
                WHERE student_id = @StudentId AND schedule_id = @ScheduleId";

            var count = await QuerySingleAsync<int>(query, new { StudentId = studentId, ScheduleId = scheduleId });
            return count > 0;
        }

        public async Task<List<EnrollmentDto>> GetActiveEnrollmentsByStudentIdAsync(int studentId)
        {
            const string query = @"
                SELECT 
                    e.id,
                    e.student_id as StudentId,
                    e.schedule_id as ScheduleId,
                    e.current_status as CurrentStatus
                FROM Enrollments e
                WHERE e.student_id = @StudentId 
                AND e.current_status = 'Active'";

            var result = await QueryAsync<EnrollmentDto>(query, new { StudentId = studentId });
            return result.ToList();
        }
    }
}
