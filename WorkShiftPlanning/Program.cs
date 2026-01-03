using WorkShiftPlanning;

Console.Clear();
Console.ForegroundColor = ConsoleColor.White;
Console.WriteLine("=== Work Shift Planning System ===\n");
Console.ForegroundColor = ConsoleColor.Gray;

// Get input parameters
string defaultYear = DateTime.Now.Year.ToString();
Console.Write($"Enter year (default: {defaultYear}): ");
var yearInput = Console.ReadLine();
int year = int.Parse(string.IsNullOrWhiteSpace(yearInput) ? defaultYear : yearInput);

string defaultMonth = "3"; // DateTime.Now.Month.ToString();
Console.Write($"Enter month (default: {defaultMonth}): ");
var monthInput = Console.ReadLine();
int month = int.Parse(string.IsNullOrWhiteSpace(monthInput) ? defaultMonth : monthInput);

string defaultTotalStaff = "20";
Console.Write($"Enter total number of staff members (default: {defaultTotalStaff}): ");
var totalStaffCountInput = Console.ReadLine();
int totalStaffCount = int.Parse(string.IsNullOrWhiteSpace(totalStaffCountInput) ? defaultTotalStaff : totalStaffCountInput);

string defaultShifts = "2";
Console.Write($"Enter number of shifts per day (default: {defaultShifts}): ");
var numberOfShiftsInput = Console.ReadLine();
int numberOfShifts = int.Parse(string.IsNullOrWhiteSpace(numberOfShiftsInput) ? defaultShifts : numberOfShiftsInput);

string defaultStaffPerShift = "1";
Console.Write($"Enter number of staff per shift (default: {defaultStaffPerShift}): ");
var staffPerShiftInput = Console.ReadLine();
int staffPerShift = int.Parse(string.IsNullOrWhiteSpace(staffPerShiftInput) ? defaultStaffPerShift : staffPerShiftInput);

// Calculate shift duration automatically to ensure 24-hour coverage
double shiftDuration = 24.0 / numberOfShifts;
Console.WriteLine($"  Calculated shift duration: {shiftDuration:F2} hours per shift");

string defaultFirstShiftStart = "8";
Console.Write($"Enter first shift start time (0-23, default: {defaultFirstShiftStart}): ");
var firstShiftStartHourInput = Console.ReadLine();
int firstShiftStartHour = int.Parse(string.IsNullOrWhiteSpace(firstShiftStartHourInput) ? defaultFirstShiftStart : firstShiftStartHourInput);

string defaultRegularStart = "8";
Console.Write($"Enter regular workday start time (0-23, default: {defaultRegularStart}): ");
var regularStartHourInput = Console.ReadLine();
int regularStartHour = int.Parse(string.IsNullOrWhiteSpace(regularStartHourInput) ? defaultRegularStart : regularStartHourInput);

string defaultStandardHours = "7.5";
Console.Write($"Enter standard workday hours (default: {defaultStandardHours}): ");
var standardHoursInput = Console.ReadLine();
double standardHours = double.Parse(string.IsNullOrWhiteSpace(standardHoursInput) ? defaultStandardHours : standardHoursInput);

string defaultMaxOnDutyHours = "36";
Console.Write($"Enter maximum total extended hours per person per month (default: {defaultMaxOnDutyHours}): ");
var maxOnDutyHoursPerMonthInput = Console.ReadLine();
double maxOnDutyHoursPerMonth = double.Parse(string.IsNullOrWhiteSpace(maxOnDutyHoursPerMonthInput) ? defaultMaxOnDutyHours : maxOnDutyHoursPerMonthInput);

// Calculate maximum work hours for the selected month
int workdaysInMonth = Enumerable.Range(1, DateTime.DaysInMonth(year, month))
    .Select(day => new DateTime(year, month, day))
    .Count(date => date.DayOfWeek != DayOfWeek.Saturday &&
                   date.DayOfWeek != DayOfWeek.Sunday);
double maxWorkHoursThisMonth = (workdaysInMonth * standardHours) + maxOnDutyHoursPerMonth;
Console.WriteLine($"  Calculated maximum work hours for {new DateTime(year, month, 1):MMMM yyyy}: {maxWorkHoursThisMonth:F2}h (Regular: {workdaysInMonth * standardHours:F2}h + Extended: {maxOnDutyHoursPerMonth:F2}h)");

string defaultRestHours = "12";
Console.Write($"Enter minimum rest hours required before shift (default: {defaultRestHours}): ");
var restHoursBeforeInput = Console.ReadLine();
double restHoursBefore = double.Parse(string.IsNullOrWhiteSpace(restHoursBeforeInput) ? defaultRestHours : restHoursBeforeInput);

string defaultRestHoursAfter = "24";
Console.Write($"Enter minimum rest hours required after shift (default: {defaultRestHoursAfter}): ");
var restHoursAfterInput = Console.ReadLine();
double restHoursAfter = double.Parse(string.IsNullOrWhiteSpace(restHoursAfterInput) ? defaultRestHoursAfter : restHoursAfterInput);

// Create schedule calculator
var calculator = new WorkShiftScheduleCalculator(
    year,
    month,
    totalStaffCount,
    numberOfShifts,
    staffPerShift,
    firstShiftStartHour,
    regularStartHour,
    shiftDuration,
    standardHours,
    maxOnDutyHoursPerMonth,
    restHoursBefore,
    restHoursAfter);

// Calculate schedule
var schedule = calculator.CalculateSchedule();

// Display results
Console.ForegroundColor = ConsoleColor.White;
Console.WriteLine($"\n=== Schedule for {new DateTime(year, month, 1):MMMM yyyy} ===\n");
Console.ForegroundColor = ConsoleColor.Gray;
Console.WriteLine($"Total Staff Members: {totalStaffCount}");
Console.WriteLine($"Regular Workday:     {regularStartHour:D2}:00 - {(regularStartHour + (int)standardHours) % 24:D2}:00 ({standardHours} hours)");
Console.WriteLine($"  All available staff (not on shift or rest) will be assigned regular work");
Console.WriteLine($"\n24-Hour Shift Coverage ({numberOfShifts} sequential shifts):");
for (int i = 0; i < numberOfShifts; i++)
{
    int startHour = (firstShiftStartHour + (int)(i * shiftDuration)) % 24;
    int endHour = (firstShiftStartHour + (int)((i + 1) * shiftDuration)) % 24;
    Console.WriteLine($"  Shift {(char)('A' + i)}: {startHour:D2}:00 - {endHour:D2}:00 ({shiftDuration}h)");
}
Console.WriteLine($"  Staff per shift: {staffPerShift}");
Console.WriteLine($"\nRest Requirements:              {restHoursBefore}h before shift, {restHoursAfter}h after shift");
Console.WriteLine($"Monthly Extended Hours Limit:   {maxOnDutyHoursPerMonth}h per person");
Console.WriteLine($"\nTotal Days:                     {schedule.TotalDays}");
Console.WriteLine($"Workdays:                       {schedule.Workdays}");
Console.WriteLine($"Weekends/Holidays:              {schedule.NonWorkdays}");
Console.WriteLine($"\nTotal Regular Hours Required:   {schedule.TotalRegularHours:F2}");
Console.WriteLine($"Total Extended Hours:           {schedule.TotalExtendedHours:F2}");

if (schedule.RestViolations.Count > 0)
{
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine($"\nWARNING: {schedule.RestViolations.Count} rest period violations detected!");
    Console.WriteLine("Some staff may not have adequate rest between shifts.");
    Console.ForegroundColor = ConsoleColor.Gray;
}

Console.ForegroundColor = ConsoleColor.White;
Console.WriteLine("\n=== Daily Breakdown ===");
Console.ForegroundColor = ConsoleColor.Gray;
foreach (var day in schedule.DailySchedules)
{
    string dayType = day.IsWorkday ? "Workday" : "Holiday/Weekend";
    Console.WriteLine($"\n{day.Date:yyyy-MM-dd} ({day.Date:ddd}) - {dayType}");

    if (day.RegularStaffAssignments.Count > 0)
    {
        Console.WriteLine($"  Regular Work Hours ({standardHours}h):");
        foreach (var assignment in day.RegularStaffAssignments.OrderBy(x => x.StaffId))
        {
            Console.WriteLine($"    - {assignment.StaffId}: {assignment.ShiftStart:HH:mm} - {assignment.ShiftEnd:HH:mm} ({assignment.Hours:F2}h)");
        }
    }

    if (day.ShiftAssignments.Count > 0)
    {
        Console.WriteLine($"  24-Hour Shift Coverage ({day.ShiftAssignments.Count} sequential shifts):");
        foreach (var assignment in day.ShiftAssignments.OrderBy(a => a.ShiftStart))
        {
            // Check if this is a weekend/holiday to determine if standard hours apply
            bool isWeekendOrHoliday = !day.IsWorkday;

            string breakdown;
            if (isWeekendOrHoliday)
            {
                // Weekend/holiday: all hours are extended hours
                breakdown = assignment.ExtendedHours > 0
                    ? $" [Extended: {assignment.ExtendedHours:F2}h]"
                    : "";
            }
            else
            {
                // Regular workday: show standard hours breakdown if there are extended hours
                breakdown = assignment.ExtendedHours > 0
                    ? $" [Std: {standardHours:F2}h + Extended: {assignment.ExtendedHours:F2}h]"
                    : "";
            }

            string shiftLabel = assignment.ShiftLabel != null ? $"{assignment.ShiftLabel} - " : "";
            Console.WriteLine($"    - {shiftLabel}{assignment.StaffId}: {assignment.ShiftStart:HH:mm} - {assignment.ShiftEnd:HH:mm} ({assignment.Hours:F2}h){breakdown}");
        }
    }
}

Console.ForegroundColor = ConsoleColor.White;
Console.WriteLine("\n=== Staff Utilization Summary ===");
Console.ForegroundColor = ConsoleColor.Gray;
Console.WriteLine($"Total Regular Work Hours: {schedule.TotalRegularHours:F2}");
Console.WriteLine($"Total Extended Hours: {schedule.TotalExtendedHours:F2}");
Console.WriteLine($"Combined Total Hours: {schedule.TotalRegularHours + schedule.TotalExtendedHours:F2}");

// Workload distribution analysis
var allStaff = schedule.StaffMembers.ToList();
if (allStaff.Any())
{
    var minHours = allStaff.Min(s => s.TotalHours);
    var maxHours = allStaff.Max(s => s.TotalHours);
    var avgHours = allStaff.Average(s => s.TotalHours);
    var avgShifts = allStaff.Average(s => s.AllAssignments.Count);

    Console.WriteLine($"\nStaff Workload Distribution:");
    Console.WriteLine($"  Min Hours: {minHours:F2}, Max Hours: {maxHours:F2}, Average: {avgHours:F2}");
    Console.WriteLine($"  Variance: {(maxHours - minHours):F2}");
    Console.WriteLine($"  Average Assignments Per Person: {avgShifts:F2}");
}

// Rest violation summary
if (schedule.RestViolations.Count > 0)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("\n=== Rest Period Violations ===");
    Console.ForegroundColor = ConsoleColor.Gray;
    foreach (var violation in schedule.RestViolations.OrderBy(x => x.StaffId).Take(10))
    {
        Console.WriteLine($"{violation.StaffId} on {violation.Date:yyyy-MM-dd}: {violation.Reason}");
        Console.WriteLine($"  Previous shift ended: {violation.PreviousShiftEnd:yyyy-MM-dd HH:mm}");
        Console.WriteLine($"  Next shift starts: {violation.NextShiftStart:yyyy-MM-dd HH:mm}");
        Console.WriteLine($"  Rest period: {violation.ActualRestHours:F2}h (required: {violation.RequiredRestHours:F2}h)");
    }
    if (schedule.RestViolations.Count > 10)
    {
        Console.WriteLine($"\n... and {schedule.RestViolations.Count - 10} more violations.");
    }
}

// Individual staff schedules
Console.ForegroundColor = ConsoleColor.White;
Console.WriteLine("\n=== Individual Staff Schedules ===");
Console.ForegroundColor = ConsoleColor.Gray;

foreach (var staff in schedule.StaffMembers.OrderBy(s => s.StaffId))
{
    Console.WriteLine($"\n{staff.StaffId}:");
    Console.WriteLine($"  Total Hours: {staff.TotalHours:F2}");
    Console.WriteLine($"  Regular Work Hours: {staff.TotalRegularHours:F2}");
    if (staff.TotalExtendedHours > 0)
    {
        Console.Write("  Extended Hours: ");
        if (staff.TotalExtendedHours > maxOnDutyHoursPerMonth)
        {
            Console.ForegroundColor = ConsoleColor.Red;
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Green;
        }
        Console.WriteLine($"{staff.TotalExtendedHours:F2}");
        Console.ForegroundColor = ConsoleColor.Gray;
    }
    Console.WriteLine($"  Total Assignments: {staff.AllAssignments.Count}");

    var violations = schedule.RestViolations.Where(v => v.StaffId == staff.StaffId).ToList();
    if (violations.Count > 0)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write("  Rest Violations: ");
        Console.WriteLine(violations.Count);
        Console.ForegroundColor = ConsoleColor.Gray;
    }

    Console.WriteLine($"  Schedule:");
    foreach (var assignment in staff.AllAssignments.OrderBy(a => a.Date).ThenBy(a => a.ShiftStart))
    {
        string assignmentType = assignment.IsRegularWork ? "Regular Work" : $"Shift {(char)('A' + assignment.ShiftNumber - 1)}";

        string breakdown;
        if (assignment.IsRegularWork)
        {
            // Regular work doesn't need breakdown
            breakdown = "";
        }
        else
        {
            // Shift work: check if it's on a weekend/holiday
            double shiftTotalHours = assignment.Hours;
            double extendedHours = assignment.ExtendedHours;
            bool isWeekendOrHolidayShift = Math.Abs(shiftTotalHours - extendedHours) < 0.01;

            if (isWeekendOrHolidayShift)
            {
                // Weekend/holiday: all hours are extended
                breakdown = extendedHours > 0
                    ? $" [Extended: {extendedHours:F2}h]"
                    : "";
            }
            else
            {
                // Regular workday: show standard hours + extended
                breakdown = extendedHours > 0
                    ? $" [Std: {standardHours:F2}h + Extended: {extendedHours:F2}h]"
                    : "";
            }
        }

        Console.WriteLine($"    {assignment.Date:yyyy-MM-dd} ({assignment.Date:ddd}) - {assignmentType}: {assignment.ShiftStart:HH:mm} - {assignment.ShiftEnd:HH:mm} ({assignment.Hours:F2}h){breakdown}");
    }
}
