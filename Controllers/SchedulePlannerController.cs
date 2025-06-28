using Microsoft.AspNetCore.Mvc;
using OnlineCourses.Models.DTOs;
using OnlineCourses.Services.Interface;

namespace OnlineCourses.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SchedulePlannerController : ControllerBase
    {
        private readonly ISchedulePlannerService _plannerService;

        public SchedulePlannerController(ISchedulePlannerService plannerService)
        {
            _plannerService = plannerService;
        }

        [HttpPost("plan")]
        public async Task<ActionResult<ApiResponse<SchedulePlannerResponseDto>>> PlanSchedule([FromBody] SchedulePlannerRequestDto request)
        {
            if (request.PreferredCourseIds == null || request.PreferredCourseIds.Count == 0 ||
                request.AvailableDays == null || request.AvailableDays.Count == 0)
            {
                return BadRequest("Preferred course IDs and available days must be provided.");
            }

            var result = await _plannerService.GetOptimalScheduleAsync(request);
            return Ok(result);
        }
    }
}
