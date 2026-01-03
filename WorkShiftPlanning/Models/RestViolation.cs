public class RestViolation
{
    public string StaffId { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public DateTime PreviousShiftEnd { get; set; }
    public DateTime NextShiftStart { get; set; }
    public double ActualRestHours { get; set; }
    public double RequiredRestHours { get; set; }
    public string Reason { get; set; } = string.Empty;
}
