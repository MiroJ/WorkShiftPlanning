namespace WorkShiftPlanning.DTOs;

public class ScheduleRequestDto
{
    public int Year { get; set; }
    public int Month { get; set; }
    public int TotalStaffCount { get; set; }
    public int NumberOfShifts { get; set; }
    public int StaffPerShift { get; set; }
    public int FirstShiftStartHour { get; set; }
    public int RegularStartHour { get; set; }
    public double ShiftDuration { get; set; }
    public double StandardHours { get; set; }
    public double MaxOnDutyHoursPerMonth { get; set; }
    public double RestHoursBefore { get; set; }
    public double RestHoursAfter { get; set; }
}

public class ScheduleResultDto
{
    public int TotalDays { get; set; }
    public int Workdays { get; set; }
    public int NonWorkdays { get; set; }
    public double TotalRegularHours { get; set; }
    public double TotalExtendedHours { get; set; }
    public List<DailyScheduleDto> DailySchedules { get; set; } = new();
    public List<StaffMemberDto> StaffMembers { get; set; } = new();
    public List<RestViolationDto> RestViolations { get; set; } = new();
    public ScheduleRequestDto Request { get; set; } = new();
}

public class DailyScheduleDto
{
    public DateTime Date { get; set; }
    public bool IsWorkday { get; set; }
    public int RegularStaffNeeded { get; set; }
    public int ShiftStaffNeeded { get; set; }
    public double RegularHours { get; set; }
    public double ExtendedHours { get; set; }
    public List<WorkAssignmentDto> RegularStaffAssignments { get; set; } = new();
    public List<WorkAssignmentDto> ShiftAssignments { get; set; } = new();
}

public class StaffMemberDto
{
    public string StaffId { get; set; } = string.Empty;
    public double TotalHours { get; set; }
    public double TotalRegularHours { get; set; }
    public double TotalExtendedHours { get; set; }
    public List<WorkAssignmentDto> AllAssignments { get; set; } = new();
}

public class WorkAssignmentDto
{
    public string StaffId { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public DateTime ShiftStart { get; set; }
    public DateTime ShiftEnd { get; set; }
    public double Hours { get; set; }
    public double ExtendedHours { get; set; }
    public string? ShiftLabel { get; set; }
    public int ShiftNumber { get; set; }
    public bool IsRegularWork { get; set; }
}

public class RestViolationDto
{
    public string StaffId { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public DateTime PreviousShiftEnd { get; set; }
    public DateTime NextShiftStart { get; set; }
    public double ActualRestHours { get; set; }
    public double RequiredRestHours { get; set; }
    public string Reason { get; set; } = string.Empty;
}
