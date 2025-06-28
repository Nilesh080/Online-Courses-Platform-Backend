using OnlineCourses.Models.DB;
using OnlineCourses.Repositories.Interface;

namespace OnlineCourses.Repositories
{
    public class StudentRepository : BaseRepository, IStudentRepository
    {
        public StudentRepository(IConfiguration configuration)
            : base(configuration.GetConnectionString("DefaultConnection"))
        {
        }

        public async Task<Student?> GetStudentByIdAsync(int studentId)
        {
            const string query = @"
                SELECT 
                    id,
                    university_id as UniversityId,
                    name,
                    email,
                    DOB
                FROM Students
                WHERE id = @StudentId";

            return await QueryFirstOrDefaultAsync<Student>(query, new { StudentId = studentId });
        }

        public async Task<Student?> GetStudentByEmailAsync(string email)
        {
            const string query = @"
                SELECT 
                    id,
                    university_id as UniversityId,
                    name,
                    email,
                    DOB
                FROM Students 
                WHERE email = @Email";

            return await QueryFirstOrDefaultAsync<Student>(query, new { Email = email });
        }
    }
}
