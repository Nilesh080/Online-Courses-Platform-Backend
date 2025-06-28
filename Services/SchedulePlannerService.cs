using OnlineCourses.Models.DTOs;
using OnlineCourses.Repositories.Interface;
using OnlineCourses.Services.Interface;

namespace OnlineCourses.Services
{
    public class SchedulePlannerService : ISchedulePlannerService
    {
        private readonly ICourseRepository _courseRepository;

        public SchedulePlannerService(ICourseRepository courseRepository)
        {
            _courseRepository = courseRepository;
        }

        public async Task<ApiResponse<SchedulePlannerResponseDto>> GetOptimalScheduleAsync(SchedulePlannerRequestDto request)
        {
            try
            {
                var selectedCourses = new List<int>();
                var dailySchedule = new Dictionary<string, List<(TimeSpan Start, TimeSpan End)>>();

                foreach (var day in request.AvailableDays)
                {
                    dailySchedule[day] = new List<(TimeSpan, TimeSpan)>();
                }

                foreach (var courseId in request.PreferredCourseIds)
                {
                    var schedules = await _courseRepository.GetSchedulesByIdAsync(courseId);
                    foreach (var schedule in schedules)
                    {
                        bool canFit = true;
                        var tempDailySchedule = dailySchedule.ToDictionary(
                            kvp => kvp.Key,
                            kvp => kvp.Value.ToList()
                        );

                        foreach (var slot in schedule.TimeSlots)
                        {
                            if (!request.AvailableDays.Contains(slot.DayOfWeek))
                            {
                                canFit = false;
                                break;
                            }

                            var list = tempDailySchedule[slot.DayOfWeek];

                            if (list.Count >= 3)
                            {
                                canFit = false;
                                break;
                            }

                            bool hasConflict = list.Any(existing =>
                                !(slot.EndTime.Add(TimeSpan.FromHours(2)) <= existing.Start ||
                                  slot.StartTime >= existing.End.Add(TimeSpan.FromHours(2)))
                            );


                            if (hasConflict)
                            {
                                canFit = false;
                                break;
                            }

                            list.Add((slot.StartTime, slot.EndTime));
                        }

                        if (canFit)
                        {
                            selectedCourses.Add(courseId);
                            dailySchedule = tempDailySchedule;
                            break; // only enroll in one schedule per course
                        }
                    }
                }

                return new ApiResponse<SchedulePlannerResponseDto>
                {
                    Success = true,
                    Message = "Schedule processed successfully",
                    Data = new SchedulePlannerResponseDto
                    {
                        SelectedCourseIds = selectedCourses,
                        Message = $"Student can attend {selectedCourses.Count} courses out of {request.PreferredCourseIds.Count}"
                    }
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<SchedulePlannerResponseDto>
                {
                    Success = false,
                    Message = "An error occurred while planning the schedule",
                    Errors = new List<string> { ex.Message }
                };
            }
        }
    }
}
