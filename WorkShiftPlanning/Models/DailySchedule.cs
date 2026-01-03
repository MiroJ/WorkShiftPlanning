public class DailySchedule
{
    public DateTime Date { get; set; }
    public bool IsWorkday { get; set; }
    public int RegularStaffNeeded { get; set; }
    public int ShiftStaffNeeded { get; set; }
    public double RegularHours { get; set; }
    public double ExtendedHours { get; set; }
    public List<WorkAssignment> RegularStaffAssignments { get; set; } = new();
    public List<WorkAssignment> ShiftAssignments { get; set; } = new();
}
