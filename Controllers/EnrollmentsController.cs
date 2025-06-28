using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineCourses.Models.DTOs;
using OnlineCourses.Services.Interface;

namespace OnlineCourses.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EnrollmentsController : ControllerBase
    {
        private readonly IEnrollmentService _enrollmentService;

        public EnrollmentsController(IEnrollmentService enrollmentService)
        {
            _enrollmentService = enrollmentService;
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ApiResponse<EnrollmentResponseDto>>> EnrollStudent([FromBody] EnrollmentRequestDto enrollmentRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<EnrollmentResponseDto>
                {
                    Success = false,
                    Message = "Invalid request data",
                    Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()
                });
            }

            var result = await _enrollmentService.EnrollStudentAsync(enrollmentRequest);

            if (!result.Success)
            {
                if (result.Message == "Student not found" || result.Message == "Schedule not found")
                {
                    return NotFound(result);
                }
                if (result.Message == "Student already enrolled" || result.Message == "Schedule is not active" || result.Message == "Schedule conflict detected")
                {
                    return BadRequest(result);
                }
                return StatusCode(500, result);
            }

            return CreatedAtAction(nameof(EnrollStudent), new { id = result.Data?.Id }, result);
        }
    }
}
