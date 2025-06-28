using OnlineCourses.Models.DB;
using OnlineCourses.Repositories.Interface;

namespace OnlineCourses.Repositories
{
    public class TeacherRepository : BaseRepository, ITeacherRepository
    {
        public TeacherRepository(IConfiguration configuration)
            : base(configuration.GetConnectionString("DefaultConnection"))
        {
        }

        public async Task<Teacher?> GetTeacherByIdAsync(int teacherId)
        {
            const string query = @"
                SELECT 
                    id,
                    university_id as UniversityId,
                    name,
                    email,
                    department
                FROM Teachers 
                WHERE id = @TeacherId";

            return await QueryFirstOrDefaultAsync<Teacher>(query, new { TeacherId = teacherId });
        }

        public async Task<Teacher?> GetTeacherByEmailAsync(string email)
        {
            const string query = @"
                SELECT 
                    id,
                    university_id as UniversityId,
                    name,
                    email,
                    department
                FROM Teachers 
                WHERE email = @Email";

            return await QueryFirstOrDefaultAsync<Teacher>(query, new { Email = email });
        }
    }
}
