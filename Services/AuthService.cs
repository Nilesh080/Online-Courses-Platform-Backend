using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using OnlineCourses.Configuration;
using OnlineCourses.Models.DTOs;
using OnlineCourses.Repositories.Interface;
using OnlineCourses.Services.Interface;

namespace OnlineCourses.Services
{
    public class AuthService : IAuthService
    {
        private readonly IStudentRepository _studentRepository;
        private readonly JwtSettings _jwtSettings;

        public AuthService(
            IStudentRepository studentRepository,
            JwtSettings jwtSettings)
        {
            _studentRepository = studentRepository;
            _jwtSettings = jwtSettings;
        }

        public async Task<ApiResponse<AuthResponseDto>> AuthenticateAsync(LoginDto loginDto)
        {
            try
            {
                var student = await _studentRepository.GetStudentByEmailAsync(loginDto.Email);
                if (student != null)
                {
                    var token = GenerateJwtToken("Student", student.Id, student.Name, student.Email);

                    return new ApiResponse<AuthResponseDto>
                    {
                        Success = true,
                        Message = "Authentication successful",
                        Data = new AuthResponseDto
                        {
                            Token = token,
                            UserType = "Student",
                            UserId = student.Id,
                            Name = student.Name,
                            Email = student.Email
                        }
                    };
                }

                return new ApiResponse<AuthResponseDto>
                {
                    Success = false,
                    Message = "Invalid credentials",
                    Errors = new List<string> { "Email is incorrect" }
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<AuthResponseDto>
                {
                    Success = false,
                    Message = "An error occurred during authentication",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public string GenerateJwtToken(string userType, int userId, string name, string email)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.SecretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                    new Claim(ClaimTypes.Name, name),
                    new Claim(ClaimTypes.Email, email),
                    new Claim(ClaimTypes.Role, userType)
                }),
                Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationInMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
