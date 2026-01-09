using Microsoft.AspNetCore.Mvc;
using WorkShiftPlanning;
using WorkShiftPlanning.DTOs;

namespace WorkShiftPlanning.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ScheduleController : ControllerBase
{
    [HttpPost("calculate")]
    public ActionResult<ScheduleResultDto> CalculateSchedule([FromBody] ScheduleRequestDto request)
    {
        try
        {
            var calculator = new WorkShiftScheduleCalculator(
                request.Year,
                request.Month,
                request.TotalStaffCount,
                request.NumberOfShifts,
                request.StaffPerShift,
                request.FirstShiftStartHour,
                request.RegularStartHour,
                request.ShiftDuration,
                request.StandardHours,
                request.MaxOnDutyHoursPerMonth,
                request.RestHoursBefore,
                request.RestHoursAfter
            );

            var schedule = calculator.CalculateSchedule();

            var result = MapToDto(schedule, request);

            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    private ScheduleResultDto MapToDto(ScheduleResult schedule, ScheduleRequestDto request)
    {
        return new ScheduleResultDto
        {
            TotalDays = schedule.TotalDays,
            Workdays = schedule.Workdays,
            NonWorkdays = schedule.NonWorkdays,
            TotalRegularHours = schedule.TotalRegularHours,
            TotalExtendedHours = schedule.TotalExtendedHours,
            DailySchedules = schedule.DailySchedules.Select(d => new DailyScheduleDto
            {
                Date = d.Date,
                IsWorkday = d.IsWorkday,
                RegularStaffNeeded = d.RegularStaffNeeded,
                ShiftStaffNeeded = d.ShiftStaffNeeded,
                RegularHours = d.RegularHours,
                ExtendedHours = d.ExtendedHours,
                RegularStaffAssignments = d.RegularStaffAssignments.Select(a => new WorkAssignmentDto
                {
                    StaffId = a.StaffId,
                    Date = a.Date,
                    ShiftStart = a.ShiftStart,
                    ShiftEnd = a.ShiftEnd,
                    Hours = a.Hours,
                    ExtendedHours = a.ExtendedHours,
                    ShiftLabel = a.ShiftLabel,
                    ShiftNumber = a.ShiftNumber,
                    IsRegularWork = a.IsRegularWork
                }).ToList(),
                ShiftAssignments = d.ShiftAssignments.Select(a => new WorkAssignmentDto
                {
                    StaffId = a.StaffId,
                    Date = a.Date,
                    ShiftStart = a.ShiftStart,
                    ShiftEnd = a.ShiftEnd,
                    Hours = a.Hours,
                    ExtendedHours = a.ExtendedHours,
                    ShiftLabel = a.ShiftLabel,
                    ShiftNumber = a.ShiftNumber,
                    IsRegularWork = a.IsRegularWork
                }).ToList()
            }).ToList(),
            StaffMembers = schedule.StaffMembers.Select(s => new StaffMemberDto
            {
                StaffId = s.StaffId,
                TotalHours = s.TotalHours,
                TotalRegularHours = s.TotalRegularHours,
                TotalExtendedHours = s.TotalExtendedHours,
                AllAssignments = s.AllAssignments.Select(a => new WorkAssignmentDto
                {
                    StaffId = a.StaffId,
                    Date = a.Date,
                    ShiftStart = a.ShiftStart,
                    ShiftEnd = a.ShiftEnd,
                    Hours = a.Hours,
                    ExtendedHours = a.ExtendedHours,
                    ShiftLabel = a.ShiftLabel,
                    ShiftNumber = a.ShiftNumber,
                    IsRegularWork = a.IsRegularWork
                }).ToList()
            }).ToList(),
            RestViolations = schedule.RestViolations.Select(v => new RestViolationDto
            {
                StaffId = v.StaffId,
                Date = v.Date,
                PreviousShiftEnd = v.PreviousShiftEnd,
                NextShiftStart = v.NextShiftStart,
                ActualRestHours = v.ActualRestHours,
                RequiredRestHours = v.RequiredRestHours,
                Reason = v.Reason
            }).ToList(),
            Request = request
        };
    }
}
