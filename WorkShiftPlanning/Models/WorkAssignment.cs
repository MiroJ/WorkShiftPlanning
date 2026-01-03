public class WorkAssignment
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
