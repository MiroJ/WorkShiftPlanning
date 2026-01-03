public class StaffMember
{
    public string StaffId { get; set; } = string.Empty;
    public List<WorkAssignment> AllAssignments { get; set; } = new();
    public double TotalHours { get; set; }
    public double TotalRegularHours { get; set; }
    public double TotalExtendedHours { get; set; }
}
