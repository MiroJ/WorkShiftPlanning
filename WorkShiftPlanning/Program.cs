// See https://aka.ms/new-console-template for more information
Console.Clear();
Console.WriteLine("=== Work Shift Planning System ===\n");

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

Console.Write("Should staff work regular hours on weekends/holidays? (y/N): ");
bool regularWorkOnHolidays = Console.ReadLine()?.Trim().ToLower() == "y";

// Create schedule calculator
var calculator = new WorkShiftScheduleCalculator(
    year,
    month,
    totalStaffCount,
    numberOfShifts,
    firstShiftStartHour,
    regularStartHour,
    shiftDuration,
    standardHours,
    maxOnDutyHoursPerMonth,
    restHoursBefore,
    restHoursAfter,
    regularWorkOnHolidays);
var schedule = calculator.CalculateSchedule();

// Display results
Console.WriteLine($"\n=== Schedule for {new DateTime(year, month, 1):MMMM yyyy} ===\n");
Console.WriteLine($"Total Staff Members: {totalStaffCount}");
Console.WriteLine($"Regular Workday: {regularStartHour:D2}:00 - {(regularStartHour + (int)standardHours) % 24:D2}:00 ({standardHours} hours)");
Console.WriteLine($"  All available staff (not on shift or rest) will be assigned regular work");
Console.WriteLine($"\n24-Hour Shift Coverage ({numberOfShifts} sequential shifts):");
for (int i = 0; i < numberOfShifts; i++)
{
    int startHour = (firstShiftStartHour + (int)(i * shiftDuration)) % 24;
    int endHour = (firstShiftStartHour + (int)((i + 1) * shiftDuration)) % 24;
    Console.WriteLine($"  Shift {i + 1}: {startHour:D2}:00 - {endHour:D2}:00 ({shiftDuration}h)");
}
Console.WriteLine($"  Staff per shift: 1");
Console.WriteLine($"\nRest Requirements: {restHoursBefore}h before shift, {restHoursAfter}h after shift");
Console.WriteLine($"Monthly Extended Hours Limit: {maxOnDutyHoursPerMonth}h per person");
Console.WriteLine($"\nTotal Days: {schedule.TotalDays}");
Console.WriteLine($"Workdays: {schedule.Workdays}");
Console.WriteLine($"Weekends/Holidays: {schedule.NonWorkdays}");
Console.WriteLine($"\nTotal Regular Hours Required: {schedule.TotalRegularHours:F2}");
Console.WriteLine($"Total Extended Hours: {schedule.TotalExtendedHours:F2}");

if (schedule.RestViolations.Count > 0)
{
    Console.WriteLine($"\n⚠ WARNING: {schedule.RestViolations.Count} rest period violations detected!");
    Console.WriteLine("Some staff may not have adequate rest between shifts.");
}

Console.WriteLine("\n=== Daily Breakdown ===");
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

Console.WriteLine("\n=== Staff Utilization Summary ===");
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
    Console.WriteLine("\n=== Rest Period Violations ===");
    foreach (var violation in schedule.RestViolations.Take(10))
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
Console.WriteLine("\n=== Individual Staff Schedules ===");

foreach (var staff in schedule.StaffMembers.OrderBy(s => s.StaffId))
{
    Console.WriteLine($"\n{staff.StaffId}:");
    Console.WriteLine($"  Total Hours: {staff.TotalHours:F2}");
    Console.WriteLine($"  Regular Work Hours: {staff.TotalRegularHours:F2}");
    if (staff.TotalExtendedHours > 0)
    {
        Console.WriteLine($"  Extended Hours: {staff.TotalExtendedHours:F2}");
    }
    Console.WriteLine($"  Total Assignments: {staff.AllAssignments.Count}");

    var violations = schedule.RestViolations.Where(v => v.StaffId == staff.StaffId).ToList();
    if (violations.Count > 0)
    {
        Console.WriteLine($"  ⚠ Rest Violations: {violations.Count}");
    }

    Console.WriteLine($"  Schedule:");
    foreach (var assignment in staff.AllAssignments.OrderBy(a => a.Date).ThenBy(a => a.ShiftStart))
    {
        string assignmentType = assignment.IsRegularWork ? "Regular Work" : $"Shift {assignment.ShiftNumber}";

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

// Work shift schedule calculator
class WorkShiftScheduleCalculator
{
    private readonly int _year;
    private readonly int _month;
    private readonly int _totalStaffCount;
    private readonly int _numberOfShifts;
    private readonly int _firstShiftStartHour;
    private readonly int _regularStartHour;
    private readonly double _shiftDuration;
    private readonly double _standardHours;
    private readonly double _maxOnDutyHoursPerMonth;
    private readonly double _restHoursBefore;
    private readonly double _restHoursAfter;
    private readonly bool _regularWorkOnHolidays;
    private readonly List<DateTime> _holidays;

    public WorkShiftScheduleCalculator(
        int year,
        int month,
        int totalStaffCount,
        int numberOfShifts,
        int firstShiftStartHour,
        int regularStartHour,
        double shiftDuration,
        double standardHours,
        double maxOnDutyHoursPerMonth,
        double restHoursBefore,
        double restHoursAfter,
        bool regularWorkOnHolidays)
    {
        _year = year;
        _month = month;
        _totalStaffCount = totalStaffCount;
        _numberOfShifts = numberOfShifts;
        _firstShiftStartHour = firstShiftStartHour;
        _regularStartHour = regularStartHour;
        _shiftDuration = shiftDuration;
        _standardHours = standardHours;
        _maxOnDutyHoursPerMonth = maxOnDutyHoursPerMonth;
        _restHoursBefore = restHoursBefore;
        _restHoursAfter = restHoursAfter;
        _regularWorkOnHolidays = regularWorkOnHolidays;
        _holidays = GetHolidays(year);
    }

    public ScheduleResult CalculateSchedule()
    {
        var daysInMonth = DateTime.DaysInMonth(_year, _month);
        var dailySchedules = new List<DailySchedule>();
        var staffMembers = new List<StaffMember>();
        var restViolations = new List<RestViolation>();

        // Initialize staff members - all with same naming scheme
        for (int i = 1; i <= _totalStaffCount; i++)
        {
            staffMembers.Add(new StaffMember
            {
                StaffId = $"STAFF-{i:D3}"
            });
        }

        // Initialize all daily schedules first
        for (int day = 1; day <= daysInMonth; day++)
        {
            var date = new DateTime(_year, _month, day);
            bool isWorkday = IsWorkday(date);

            dailySchedules.Add(new DailySchedule
            {
                Date = date,
                IsWorkday = isWorkday
            });
        }

        double totalRegularHours = 0;
        double totalExtendedHours = 0;

        // PHASE 1: Assign 24-hour shift coverage for the entire month FIRST
        // This must be done first because shifts are mandatory 24/7 coverage
        int[] shiftStaffRotation = new int[_numberOfShifts];

        for (int day = 1; day <= daysInMonth; day++)
        {
            var dailySchedule = dailySchedules[day - 1];
            var date = dailySchedule.Date;
            bool isWorkday = dailySchedule.IsWorkday;

            // Shifts are scheduled EVERY day for 24-hour coverage
            if (_numberOfShifts > 0)
            {
                dailySchedule.ShiftStaffNeeded = _numberOfShifts;

                double shiftHoursPerPerson = _shiftDuration;
                double extendedHoursPerPerson = 0;

                bool isWeekendOrHoliday = !isWorkday;

                if (isWeekendOrHoliday)
                {
                    // Weekend/holiday shifts: ALL hours count as extended hours
                    extendedHoursPerPerson = shiftHoursPerPerson;
                }
                else
                {
                    // Regular workday shifts: hours beyond standard work hours count as extended
                    if (shiftHoursPerPerson > _standardHours)
                    {
                        double overtimeHours = shiftHoursPerPerson - _standardHours;
                        extendedHoursPerPerson = overtimeHours;
                    }
                }

                dailySchedule.ExtendedHours = extendedHoursPerPerson;

                for (int shiftIndex = 0; shiftIndex < _numberOfShifts; shiftIndex++)
                {
                    double shiftStartOffset = shiftIndex * _shiftDuration;
                    var baseShiftStart = new DateTime(date.Year, date.Month, date.Day, _firstShiftStartHour, 0, 0);
                    var shiftStart = baseShiftStart.AddHours(shiftStartOffset);
                    var shiftEnd = shiftStart.AddHours(shiftHoursPerPerson);

                    bool assigned = false;
                    int attempts = 0;
                    int maxAttempts = _totalStaffCount * 2;

                    while (!assigned && attempts < maxAttempts)
                    {
                        var staffIndex = (shiftStaffRotation[shiftIndex] + attempts) % _totalStaffCount;
                        var staff = staffMembers[staffIndex];

                        bool alreadyAssignedShiftToday = dailySchedule.ShiftAssignments.Any(a => a.StaffId == staff.StaffId);

                        if (!alreadyAssignedShiftToday)
                        {
                            var canWork = CanStaffWorkShift(staff, shiftStart, shiftEnd, date, out var violation);

                            if (canWork || attempts >= _totalStaffCount)
                            {
                                var assignment = new WorkAssignment
                                {
                                    StaffId = staff.StaffId,
                                    Date = date,
                                    ShiftStart = shiftStart,
                                    ShiftEnd = shiftEnd,
                                    Hours = shiftHoursPerPerson,
                                    ExtendedHours = extendedHoursPerPerson,
                                    ShiftLabel = $"Shift {shiftIndex + 1}",
                                    ShiftNumber = shiftIndex + 1,
                                    IsRegularWork = false
                                };

                                dailySchedule.ShiftAssignments.Add(assignment);
                                staff.AllAssignments.Add(assignment);

                                if (!canWork && violation != null)
                                {
                                    restViolations.Add(violation);
                                }

                                shiftStaffRotation[shiftIndex] = (staffIndex + 1) % _totalStaffCount;
                                assigned = true;
                            }
                        }

                        attempts++;
                    }

                    if (assigned)
                    {
                        totalExtendedHours += extendedHoursPerPerson;
                    }
                }
            }
        }

        // PHASE 2: Assign regular work, avoiding staff with shifts that would violate rest periods
        int regularStaffRotation = 0;
        
        for (int day = 1; day <= daysInMonth; day++)
        {
            var dailySchedule = dailySchedules[day - 1];
            var date = dailySchedule.Date;
            bool isWorkday = dailySchedule.IsWorkday;

            bool regularWorkScheduled = isWorkday || _regularWorkOnHolidays;

            if (regularWorkScheduled && _totalStaffCount > 0)
            {
                dailySchedule.RegularStaffNeeded = _totalStaffCount;
                dailySchedule.RegularHours = _standardHours;

                int assignedCount = 0;
                int attempts = 0;
                int maxAttempts = _totalStaffCount * 2;
                
                var regularWorkStart = new DateTime(date.Year, date.Month, date.Day, _regularStartHour, 0, 0);
                var regularWorkEnd = regularWorkStart.AddHours(_standardHours);

                while (assignedCount < _totalStaffCount && attempts < maxAttempts)
                {
                    var staffIndex = (regularStaffRotation + attempts) % _totalStaffCount;
                    var staff = staffMembers[staffIndex];

                    // Check if staff already has regular work today
                    bool alreadyAssignedRegularToday = dailySchedule.RegularStaffAssignments.Any(a => a.StaffId == staff.StaffId);
                    
                    if (!alreadyAssignedRegularToday)
                    {
                        bool canWork = true;
                        
                        // Check if staff has a shift today
                        bool hasShiftToday = dailySchedule.ShiftAssignments.Any(a => a.StaffId == staff.StaffId);
                        if (hasShiftToday)
                        {
                            canWork = false;
                        }
                        else
                        {
                            // Check rest period after previous shift
                            var previousShift = staff.AllAssignments
                                .Where(a => !a.IsRegularWork && a.ShiftEnd <= regularWorkStart)
                                .OrderByDescending(a => a.ShiftEnd)
                                .FirstOrDefault();
                                
                            if (previousShift != null)
                            {
                                var hoursSinceShiftEnded = (regularWorkStart - previousShift.ShiftEnd).TotalHours;
                                if (hoursSinceShiftEnded < _restHoursAfter)
                                {
                                    canWork = false;
                                }
                            }

                            // Check rest period before next shift
                            var nextShift = staff.AllAssignments
                                .Where(a => !a.IsRegularWork && a.ShiftStart >= regularWorkEnd)
                                .OrderBy(a => a.ShiftStart)
                                .FirstOrDefault();
                                
                            if (nextShift != null)
                            {
                                var hoursUntilShiftStarts = (nextShift.ShiftStart - regularWorkEnd).TotalHours;
                                if (hoursUntilShiftStarts < _restHoursBefore)
                                {
                                    canWork = false;
                                }
                            }
                        }

                        if (canWork)
                        {
                            // Assign regular work
                            var assignment = new WorkAssignment
                            {
                                StaffId = staff.StaffId,
                                Date = date,
                                ShiftStart = regularWorkStart,
                                ShiftEnd = regularWorkEnd,
                                Hours = _standardHours,
                                IsRegularWork = true
                            };

                            dailySchedule.RegularStaffAssignments.Add(assignment);
                            staff.AllAssignments.Add(assignment);
                            assignedCount++;
                            
                            if (assignedCount == 1)
                            {
                                regularStaffRotation = (staffIndex + 1) % _totalStaffCount;
                            }
                        }
                    }

                    attempts++;
                }

                totalRegularHours += dailySchedule.RegularStaffAssignments.Count * _standardHours;
            }
        }

        // Calculate totals for each staff member
        foreach (var staff in staffMembers)
        {
            staff.TotalHours = staff.AllAssignments.Sum(a => a.Hours);
            staff.TotalRegularHours = staff.AllAssignments.Where(a => a.IsRegularWork).Sum(a => a.Hours);
            staff.TotalExtendedHours = staff.AllAssignments.Where(a => !a.IsRegularWork).Sum(a => a.ExtendedHours);
        }

        var workdays = dailySchedules.Count(d => d.IsWorkday);

        return new ScheduleResult
        {
            TotalDays = daysInMonth,
            Workdays = workdays,
            NonWorkdays = daysInMonth - workdays,
            TotalRegularHours = totalRegularHours,
            TotalExtendedHours = totalExtendedHours,
            DailySchedules = dailySchedules,
            StaffMembers = staffMembers,
            RestViolations = restViolations
        };
    }

    private bool CanStaffWorkShift(StaffMember staff, DateTime shiftStart, DateTime shiftEnd, DateTime currentDate, out RestViolation? violation)
    {
        violation = null;

        // Check monthly extended hours limit (this includes all non-regular work)
        double currentExtendedHours = staff.AllAssignments
            .Where(a => !a.IsRegularWork)
            .Sum(a => a.ExtendedHours);

        // Calculate extended hours for this potential shift
        double shiftHoursPerPerson = _shiftDuration;
        double potentialExtendedHours = 0;

        // Check if this is a weekend/holiday
        bool isWeekendOrHoliday = !IsWorkday(currentDate);

        if (isWeekendOrHoliday)
        {
            // Weekend/holiday shifts: all hours are extended hours
            potentialExtendedHours = shiftHoursPerPerson;
        }
        else
        {
            // Regular workday shifts: only hours beyond standard are extended
            if (shiftHoursPerPerson > _standardHours)
            {
                double overtimeHours = shiftHoursPerPerson - _standardHours;
                potentialExtendedHours = overtimeHours;
            }
        }

        if (currentExtendedHours + potentialExtendedHours > _maxOnDutyHoursPerMonth)
        {
            violation = new RestViolation
            {
                StaffId = staff.StaffId,
                Date = currentDate,
                PreviousShiftEnd = DateTime.MinValue,
                NextShiftStart = shiftStart,
                ActualRestHours = currentExtendedHours + potentialExtendedHours,
                RequiredRestHours = _maxOnDutyHoursPerMonth,
                Reason = $"Would exceed monthly extended hours limit ({currentExtendedHours:F2}h + {potentialExtendedHours:F2}h = {currentExtendedHours + potentialExtendedHours:F2}h > {_maxOnDutyHoursPerMonth:F2}h)"
            };
            return false;
        }

        // Check rest period after previous shift assignment
        var previousShiftAssignment = staff.AllAssignments
            .Where(a => !a.IsRegularWork)
            .OrderByDescending(a => a.ShiftEnd)
            .FirstOrDefault();
            
        if (previousShiftAssignment != null)
        {
            var hoursSinceLast = (shiftStart - previousShiftAssignment.ShiftEnd).TotalHours;

            if (hoursSinceLast < _restHoursAfter)
            {
                violation = new RestViolation
                {
                    StaffId = staff.StaffId,
                    Date = currentDate,
                    PreviousShiftEnd = previousShiftAssignment.ShiftEnd,
                    NextShiftStart = shiftStart,
                    ActualRestHours = hoursSinceLast,
                    RequiredRestHours = _restHoursAfter,
                    Reason = "Insufficient rest after previous shift assignment"
                };
                return false;
            }
        }

        return true;
    }

    private bool IsWorkday(DateTime date)
    {
        if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
            return false;

        if (_holidays.Any(h => h.Date == date.Date))
            return false;

        return true;
    }

    private List<DateTime> GetHolidays(int year)
    {
        var holidays = new List<DateTime>
        {
            new DateTime(year, 1, 1),
            new DateTime(year, 7, 4),
            new DateTime(year, 12, 25),
        };

        holidays.Add(GetLastMondayOfMonth(year, 5));
        holidays.Add(GetFirstMondayOfMonth(year, 9));
        holidays.Add(GetNthDayOfWeek(year, 11, DayOfWeek.Thursday, 4));

        return holidays;
    }

    private DateTime GetLastMondayOfMonth(int year, int month)
    {
        var lastDay = new DateTime(year, month, DateTime.DaysInMonth(year, month));
        while (lastDay.DayOfWeek != DayOfWeek.Monday)
            lastDay = lastDay.AddDays(-1);
        return lastDay;
    }

    private DateTime GetFirstMondayOfMonth(int year, int month)
    {
        var firstDay = new DateTime(year, month, 1);
        while (firstDay.DayOfWeek != DayOfWeek.Monday)
            firstDay = firstDay.AddDays(1);
        return firstDay;
    }

    private DateTime GetNthDayOfWeek(int year, int month, DayOfWeek dayOfWeek, int occurrence)
    {
        var firstDay = new DateTime(year, month, 1);
        int count = 0;

        for (int day = 1; day <= DateTime.DaysInMonth(year, month); day++)
        {
            var date = new DateTime(year, month, day);
            if (date.DayOfWeek == dayOfWeek)
            {
                count++;
                if (count == occurrence)
                    return date;
            }
        }

        return firstDay;
    }
}

// Result models
class ScheduleResult
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

class DailySchedule
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

class WorkAssignment
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

class StaffMember
{
    public string StaffId { get; set; } = string.Empty;
    public List<WorkAssignment> AllAssignments { get; set; } = new();
    public double TotalHours { get; set; }
    public double TotalRegularHours { get; set; }
    public double TotalExtendedHours { get; set; }
}

class RestViolation
{
    public string StaffId { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public DateTime PreviousShiftEnd { get; set; }
    public DateTime NextShiftStart { get; set; }
    public double ActualRestHours { get; set; }
    public double RequiredRestHours { get; set; }
    public string Reason { get; set; } = string.Empty;
}
