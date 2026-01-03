namespace WorkShiftPlanning;

/// <summary>
/// Calculates work shift schedules for staff members over a specified month.
/// Handles both regular work assignments and 24-hour shift coverage with rest period validation.
/// </summary>
public class WorkShiftScheduleCalculator(
    int year,
    int month,
    int totalStaffCount,
    int numberOfShifts,
    int staffPerShift,
    int firstShiftStartHour,
    int regularStartHour,
    double shiftDuration,
    double standardHours,
    double maxOnDutyHoursPerMonth,
    double restHoursBefore,
    double restHoursAfter)
{
    public ScheduleResult CalculateSchedule()
    {
        var daysInMonth = DateTime.DaysInMonth(year, month);
        var dailySchedules = new List<DailySchedule>();
        var staffMembers = new List<StaffMember>();
        var restViolations = new List<RestViolation>();

        // Initialize staff members - all with same naming scheme
        for (int i = 1; i <= totalStaffCount; i++)
        {
            staffMembers.Add(new StaffMember
            {
                StaffId = $"STAFF-{i:D3}"
            });
        }

        // Initialize all daily schedules first
        for (int day = 1; day <= daysInMonth; day++)
        {
            var date = new DateTime(year, month, day);
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
        int[] shiftStaffRotation = new int[numberOfShifts];

        for (int day = 1; day <= daysInMonth; day++)
        {
            var dailySchedule = dailySchedules[day - 1];
            var date = dailySchedule.Date;
            bool isWorkday = dailySchedule.IsWorkday;

            // Shifts are scheduled EVERY day for 24-hour coverage
            if (numberOfShifts > 0)
            {
                dailySchedule.ShiftStaffNeeded = numberOfShifts * staffPerShift;

                double shiftHoursPerPerson = shiftDuration;
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
                    if (shiftHoursPerPerson > standardHours)
                    {
                        double overtimeHours = shiftHoursPerPerson - standardHours;
                        extendedHoursPerPerson = overtimeHours;
                    }
                }

                dailySchedule.ExtendedHours = extendedHoursPerPerson;

                for (int shiftIndex = 0; shiftIndex < numberOfShifts; shiftIndex++)
                {
                    double shiftStartOffset = shiftIndex * shiftDuration;
                    var baseShiftStart = new DateTime(date.Year, date.Month, date.Day, firstShiftStartHour, 0, 0);
                    var shiftStart = baseShiftStart.AddHours(shiftStartOffset);
                    var shiftEnd = shiftStart.AddHours(shiftHoursPerPerson);

                    // Assign staffPerShift number of staff to this shift
                    for (int staffSlot = 0; staffSlot < staffPerShift; staffSlot++)
                    {
                        bool assigned = false;
                        int attempts = 0;
                        int maxAttempts = totalStaffCount * 2;

                        while (!assigned && attempts < maxAttempts)
                        {
                            var staffIndex = (shiftStaffRotation[shiftIndex] + attempts) % totalStaffCount;
                            var staff = staffMembers[staffIndex];

                            bool alreadyAssignedShiftToday = dailySchedule.ShiftAssignments.Any(a => a.StaffId == staff.StaffId);

                            if (!alreadyAssignedShiftToday)
                            {
                                var canWork = CanStaffWorkShift(staff, shiftStart, shiftEnd, date, out var violation);

                                if (canWork || attempts >= totalStaffCount)
                                {
                                    var assignment = new WorkAssignment
                                    {
                                        StaffId = staff.StaffId,
                                        Date = date,
                                        ShiftStart = shiftStart,
                                        ShiftEnd = shiftEnd,
                                        Hours = shiftHoursPerPerson,
                                        ExtendedHours = extendedHoursPerPerson,
                                        ShiftLabel = $"Shift {(char)('A' + shiftIndex)}",
                                        ShiftNumber = shiftIndex + 1,
                                        IsRegularWork = false
                                    };

                                    dailySchedule.ShiftAssignments.Add(assignment);
                                    staff.AllAssignments.Add(assignment);

                                    if (!canWork && violation != null)
                                    {
                                        restViolations.Add(violation);
                                    }

                                    shiftStaffRotation[shiftIndex] = (staffIndex + 1) % totalStaffCount;
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
        }

        // PHASE 2: Assign regular work, avoiding staff with shifts that would violate rest periods
        int regularStaffRotation = 0;

        for (int day = 1; day <= daysInMonth; day++)
        {
            var dailySchedule = dailySchedules[day - 1];
            var date = dailySchedule.Date;

            if (dailySchedule.IsWorkday && totalStaffCount > 0)
            {
                dailySchedule.RegularStaffNeeded = totalStaffCount;
                dailySchedule.RegularHours = standardHours;

                int assignedCount = 0;
                int attempts = 0;
                int maxAttempts = totalStaffCount * 2;

                var regularWorkStart = new DateTime(date.Year, date.Month, date.Day, regularStartHour, 0, 0);
                var regularWorkEnd = regularWorkStart.AddHours(standardHours);

                while (assignedCount < totalStaffCount && attempts < maxAttempts)
                {
                    var staffIndex = (regularStaffRotation + attempts) % totalStaffCount;
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
                                if (hoursSinceShiftEnded < restHoursAfter)
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
                                if (hoursUntilShiftStarts < restHoursBefore)
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
                                Hours = standardHours,
                                IsRegularWork = true
                            };

                            dailySchedule.RegularStaffAssignments.Add(assignment);
                            staff.AllAssignments.Add(assignment);
                            assignedCount++;

                            if (assignedCount == 1)
                            {
                                regularStaffRotation = (staffIndex + 1) % totalStaffCount;
                            }
                        }
                    }

                    attempts++;
                }

                totalRegularHours += dailySchedule.RegularStaffAssignments.Count * standardHours;
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
        double shiftHoursPerPerson = shiftDuration;
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
            if (shiftHoursPerPerson > standardHours)
            {
                double overtimeHours = shiftHoursPerPerson - standardHours;
                potentialExtendedHours = overtimeHours;
            }
        }

        if (currentExtendedHours + potentialExtendedHours > maxOnDutyHoursPerMonth)
        {
            violation = new RestViolation
            {
                StaffId = staff.StaffId,
                Date = currentDate,
                PreviousShiftEnd = DateTime.MinValue,
                NextShiftStart = shiftStart,
                ActualRestHours = currentExtendedHours + potentialExtendedHours,
                RequiredRestHours = maxOnDutyHoursPerMonth,
                Reason = $"Would exceed monthly extended hours limit ({currentExtendedHours:F2}h + {potentialExtendedHours:F2}h = {currentExtendedHours + potentialExtendedHours:F2}h > {maxOnDutyHoursPerMonth:F2}h)"
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

            if (hoursSinceLast < restHoursAfter)
            {
                violation = new RestViolation
                {
                    StaffId = staff.StaffId,
                    Date = currentDate,
                    PreviousShiftEnd = previousShiftAssignment.ShiftEnd,
                    NextShiftStart = shiftStart,
                    ActualRestHours = hoursSinceLast,
                    RequiredRestHours = restHoursAfter,
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

        if (DateTimeExtensions.GetHolidays(date.Year).Any(h => h.Date == date.Date))
            return false;

        return true;
    }
}