using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineCourses.Models.DTOs;
using OnlineCourses.Services.Interface;

namespace OnlineCourses.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CoursesController : ControllerBase
    {
        private readonly ICourseService _courseService;

        public CoursesController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<CourseDto>>>> GetAll()
        {
            var result = await _courseService.GetAllAsync();

            if (!result.Success)
            {
                return StatusCode(500, result);
            }

            return Ok(result);
        }

        [HttpGet("{Id}/schedules")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<IEnumerable<ScheduleDto>>>> GetSchedulesByCourse([FromRoute] int Id)
        {
            if (Id <= 0)
            {
                return BadRequest(new ApiResponse<IEnumerable<ScheduleDto>>
                {
                    Success = false,
                    Message = "Invalid course ID",
                    Errors = new List<string> { "Course ID must be greater than 0" }
                });
            }

            var result = await _courseService.GetSchedulesByIdAsync(Id);

            if (!result.Success)
            {
                if (result.Message == "Course not found")
                {
                    return NotFound(result);
                }
                return StatusCode(500, result);
            }

            return Ok(result);
        }
    }
}
