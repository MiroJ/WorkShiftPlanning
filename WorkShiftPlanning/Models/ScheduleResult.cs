public class ScheduleResult
{
    public int TotalDays { get; set; }
    public int Workdays { get; set; }
    public int NonWorkdays { get; set; }
    public double TotalRegularHours { get; set; }
    public double TotalExtendedHours { get; set; }
    public List<DailySchedule> DailySchedules { get; set; } = new();
    public List<StaffMember> StaffMembers { get; set; } = new();
    public List<RestViolation> RestViolations { get; set; } = new();
}
