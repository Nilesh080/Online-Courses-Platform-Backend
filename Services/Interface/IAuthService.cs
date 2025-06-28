using OnlineCourses.Models.DTOs;

namespace OnlineCourses.Services.Interface
{
    public interface IAuthService
    {
        Task<ApiResponse<AuthResponseDto>> AuthenticateAsync(LoginDto loginDto);
        string GenerateJwtToken(string userType, int userId, string name, string email);
    }
}
