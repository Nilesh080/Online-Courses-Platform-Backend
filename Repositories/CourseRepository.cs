using Dapper;
using OnlineCourses.Models.DB;
using OnlineCourses.Models.DTOs;
using OnlineCourses.Repositories.Interface;

namespace OnlineCourses.Repositories
{
    public class CourseRepository : BaseRepository, ICourseRepository
    {
        public CourseRepository(IConfiguration configuration)
            : base(configuration.GetConnectionString("DefaultConnection"))
        {
        }

        public async Task<IEnumerable<CourseDto>> GetAllAsync()
        {
            const string query = @"
                SELECT 
                    c.id,
                    c.name,
                    c.description,
                    c.total_hours as TotalHours,
                    u.name as UniversityName
                FROM Courses c
                INNER JOIN University u ON c.university_id = u.id
                ORDER BY c.name";

            return await QueryAsync<CourseDto>(query);
        }

        public async Task<Course?> GetByIdAsync(int courseId)
        {
            const string query = @"
                SELECT 
                    id,
                    university_id as UniversityId,
                    name,
                    description,
                    total_hours as TotalHours
                FROM Courses 
                WHERE id = @CourseId";

            return await QueryFirstOrDefaultAsync<Course>(query, new { CourseId = courseId });
        }

        public async Task<IEnumerable<ScheduleDto>> GetSchedulesByIdAsync(int courseId)
        {
            const string scheduleQuery = @"
                SELECT 
                    s.id,
                    s.course_id as CourseId,
                    c.name as CourseName,
                    t.name as TeacherName,
                    t.email as TeacherEmail,
                    t.department as Department,
                    s.is_active as IsActive
                FROM Schedules s
                INNER JOIN Courses c ON s.course_id = c.id
                INNER JOIN Teachers t ON s.teacher_id = t.id
                WHERE s.course_id = @CourseId AND s.is_active = 1
                ORDER BY t.name";

            const string timeSlotQuery = @"
                SELECT 
                    day_of_week as DayOfWeek,
                    start_time as StartTime,
                    end_time as EndTime
                FROM Schedule_Time_Slots
                WHERE schedule_id = @ScheduleId
                ORDER BY 
                    CASE day_of_week
                        WHEN 'Monday' THEN 1
                        WHEN 'Tuesday' THEN 2
                        WHEN 'Wednesday' THEN 3
                        WHEN 'Thursday' THEN 4
                        WHEN 'Friday' THEN 5
                        WHEN 'Saturday' THEN 6
                        WHEN 'Sunday' THEN 7
                    END,
                    start_time";

            return await ExecuteWithConnectionAsync(async connection =>
            {
                var schedules = await connection.QueryAsync<ScheduleDto>(scheduleQuery, new { CourseId = courseId });
                var scheduleList = schedules.ToList();

                var loadTimeSlotsTasks = scheduleList.Select(async schedule =>
                {
                    var timeSlots = await connection.QueryAsync<TimeSlotDto>(timeSlotQuery, new { ScheduleId = schedule.Id });
                    schedule.TimeSlots = timeSlots.ToList();
                });

                await Task.WhenAll(loadTimeSlotsTasks);
                return scheduleList.AsEnumerable();
            });
        }

        public async Task<Schedule?> GetScheduleByIdAsync(int scheduleId)
        {
            const string query = @"
                SELECT 
                    id,
                    course_id as CourseId,
                    teacher_id as TeacherId,
                    is_active as IsActive
                FROM Schedules 
                WHERE id = @ScheduleId";

            return await QueryFirstOrDefaultAsync<Schedule>(query, new { ScheduleId = scheduleId });
        }

        public async Task<List<ScheduleDto>> GetSchedulesByIdsAsync(List<int> scheduleIds)
        {
            if (!scheduleIds.Any())
                return new List<ScheduleDto>();

            const string scheduleQuery = @"
                SELECT 
                    s.id,
                    s.course_id as CourseId,
                    c.name as CourseName,
                    t.name as TeacherName,
                    t.email as TeacherEmail,
                    t.department as Department,
                    s.is_active as IsActive
                FROM Schedules s
                INNER JOIN Courses c ON s.course_id = c.id
                INNER JOIN Teachers t ON s.teacher_id = t.id
                WHERE s.id IN @ScheduleIds";

            const string timeSlotQuery = @"
                SELECT 
                    day_of_week as DayOfWeek,
                    start_time as StartTime,
                    end_time as EndTime
                FROM Schedule_Time_Slots
                WHERE schedule_id = @ScheduleId
                ORDER BY 
                    CASE day_of_week
                        WHEN 'Monday' THEN 1
                        WHEN 'Tuesday' THEN 2
                        WHEN 'Wednesday' THEN 3
                        WHEN 'Thursday' THEN 4
                        WHEN 'Friday' THEN 5
                        WHEN 'Saturday' THEN 6
                        WHEN 'Sunday' THEN 7
                    END,
                    start_time";

            return await ExecuteWithConnectionAsync(async connection =>
            {
                var schedules = await connection.QueryAsync<ScheduleDto>(scheduleQuery, new { ScheduleIds = scheduleIds });
                var scheduleList = schedules.ToList();

                var loadTasks = scheduleList.Select(async schedule =>
                {
                    var timeSlots = await connection.QueryAsync<TimeSlotDto>(timeSlotQuery, new { ScheduleId = schedule.Id });
                    schedule.TimeSlots = timeSlots.ToList();
                });

                await Task.WhenAll(loadTasks);
                return scheduleList;
            });
        }
    }
}