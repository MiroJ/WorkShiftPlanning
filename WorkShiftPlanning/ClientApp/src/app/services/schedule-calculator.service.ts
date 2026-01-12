import { Injectable } from '@angular/core';
import {
  ScheduleRequest,
  ScheduleResult,
  DailySchedule,
  StaffMember,
  WorkAssignment,
  RestViolation
} from '../models/schedule.models';
import { HolidayService } from './holiday.service';

/**
 * Local schedule calculator service - runs entirely in the browser
 * Converts the C# WorkShiftScheduleCalculator logic to TypeScript
 */
@Injectable({
  providedIn: 'root'
})
export class ScheduleCalculatorService {

  /**
   * Calculate the complete work shift schedule
   */
  calculateSchedule(request: ScheduleRequest): ScheduleResult {
    const daysInMonth = new Date(request.year, request.month, 0).getDate();
    const dailySchedules: DailySchedule[] = [];
    const staffMembers: StaffMember[] = [];
    const restViolations: RestViolation[] = [];

    // Initialize staff members
    for (let i = 1; i <= request.totalStaffCount; i++) {
      staffMembers.push({
        staffId: `STAFF-${i.toString().padStart(3, '0')}`,
        totalHours: 0,
        totalRegularHours: 0,
        totalExtendedHours: 0,
        allAssignments: []
      });
    }

    // Initialize all daily schedules
    for (let day = 1; day <= daysInMonth; day++) {
      const date = new Date(request.year, request.month - 1, day);
      const isWorkday = HolidayService.isWorkday(date);

      dailySchedules.push({
        date: date.toISOString(),
        isWorkday: isWorkday,
        regularStaffNeeded: 0,
        shiftStaffNeeded: 0,
        regularHours: 0,
        extendedHours: 0,
        regularStaffAssignments: [],
        shiftAssignments: []
      });
    }

    let totalRegularHours = 0;
    let totalExtendedHours = 0;

    // PHASE 1: Assign 24-hour shift coverage for the entire month
    for (let day = 1; day <= daysInMonth; day++) {
      const dailySchedule = dailySchedules[day - 1];
      const date = new Date(request.year, request.month - 1, day);
      const isWorkday = dailySchedule.isWorkday;

      if (request.numberOfShifts > 0) {
        dailySchedule.shiftStaffNeeded = request.numberOfShifts * request.staffPerShift;

        const shiftHoursPerPerson = request.shiftDuration;
        let extendedHoursPerPerson = 0;

        const isWeekendOrHoliday = !isWorkday;

        if (isWeekendOrHoliday) {
          // Weekend/holiday shifts: ALL hours count as extended hours
          extendedHoursPerPerson = shiftHoursPerPerson;
        } else {
          // Regular workday shifts: hours beyond standard work hours count as extended
          if (shiftHoursPerPerson > request.standardHours) {
            extendedHoursPerPerson = shiftHoursPerPerson - request.standardHours;
          }
        }

        dailySchedule.extendedHours = extendedHoursPerPerson;

        for (let shiftIndex = 0; shiftIndex < request.numberOfShifts; shiftIndex++) {
          const shiftStartOffset = shiftIndex * request.shiftDuration;
          const shiftStart = new Date(date.getFullYear(), date.getMonth(), date.getDate(), request.firstShiftStartHour, 0, 0);
          shiftStart.setHours(shiftStart.getHours() + shiftStartOffset);
          
          const shiftEnd = new Date(shiftStart);
          shiftEnd.setHours(shiftEnd.getHours() + shiftHoursPerPerson);

          // Assign staffPerShift number of staff to this shift
          for (let staffSlot = 0; staffSlot < request.staffPerShift; staffSlot++) {
            // Sort staff by priority
            const eligibleStaff = staffMembers
              .map(s => ({
                staff: s,
                extendedHours: s.allAssignments
                  .filter(a => !a.isRegularWork)
                  .reduce((sum, a) => sum + a.extendedHours, 0),
                thisShiftCount: s.allAssignments
                  .filter(a => !a.isRegularWork && a.shiftNumber === shiftIndex + 1).length,
                alreadyAssignedToday: dailySchedule.shiftAssignments
                  .some(a => a.staffId === s.staffId)
              }))
              .filter(x => !x.alreadyAssignedToday)
              .sort((a, b) => {
                if (a.extendedHours !== b.extendedHours) {
                  return a.extendedHours - b.extendedHours;
                }
                return a.thisShiftCount - b.thisShiftCount;
              });

            let assigned = false;

            for (const candidate of eligibleStaff) {
              const violation = this.canStaffWorkShift(
                candidate.staff,
                shiftStart,
                shiftEnd,
                date,
                request
              );

              if (!violation) {
                const assignment: WorkAssignment = {
                  staffId: candidate.staff.staffId,
                  date: date.toISOString(),
                  shiftStart: shiftStart.toISOString(),
                  shiftEnd: shiftEnd.toISOString(),
                  hours: shiftHoursPerPerson,
                  extendedHours: extendedHoursPerPerson,
                  shiftLabel: `Shift ${String.fromCharCode(65 + shiftIndex)}`,
                  shiftNumber: shiftIndex + 1,
                  isRegularWork: false
                };

                dailySchedule.shiftAssignments.push(assignment);
                candidate.staff.allAssignments.push(assignment);
                assigned = true;
                break;
              }
            }

            // Fallback: if no one can work, assign to least-violating staff
            if (!assigned && eligibleStaff.length > 0) {
              const candidate = eligibleStaff[0];
              const assignment: WorkAssignment = {
                staffId: candidate.staff.staffId,
                date: date.toISOString(),
                shiftStart: shiftStart.toISOString(),
                shiftEnd: shiftEnd.toISOString(),
                hours: shiftHoursPerPerson,
                extendedHours: extendedHoursPerPerson,
                shiftLabel: `Shift ${String.fromCharCode(65 + shiftIndex)}`,
                shiftNumber: shiftIndex + 1,
                isRegularWork: false
              };

              dailySchedule.shiftAssignments.push(assignment);
              candidate.staff.allAssignments.push(assignment);

              // Record the violation
              const violation = this.canStaffWorkShift(
                candidate.staff,
                shiftStart,
                shiftEnd,
                date,
                request
              );
              if (violation) {
                restViolations.push(violation);
              }

              assigned = true;
            }

            if (assigned) {
              totalExtendedHours += extendedHoursPerPerson;
            }
          }
        }
      }
    }

    // PHASE 2: Assign regular work
    let regularStaffRotation = 0;

    for (let day = 1; day <= daysInMonth; day++) {
      const dailySchedule = dailySchedules[day - 1];
      const date = new Date(request.year, request.month - 1, day);

      if (dailySchedule.isWorkday && request.totalStaffCount > 0) {
        dailySchedule.regularStaffNeeded = request.totalStaffCount;
        dailySchedule.regularHours = request.standardHours;

        let assignedCount = 0;
        let attempts = 0;
        const maxAttempts = request.totalStaffCount * 2;

        const regularWorkStart = new Date(date.getFullYear(), date.getMonth(), date.getDate(), request.regularStartHour, 0, 0);
        const regularWorkEnd = new Date(regularWorkStart);
        regularWorkEnd.setHours(regularWorkEnd.getHours() + request.standardHours);

        while (assignedCount < request.totalStaffCount && attempts < maxAttempts) {
          const staffIndex = (regularStaffRotation + attempts) % request.totalStaffCount;
          const staff = staffMembers[staffIndex];

          const alreadyAssignedRegularToday = dailySchedule.regularStaffAssignments
            .some(a => a.staffId === staff.staffId);

          if (!alreadyAssignedRegularToday) {
            let canWork = true;

            // Check if staff has a shift today
            const hasShiftToday = dailySchedule.shiftAssignments
              .some(a => a.staffId === staff.staffId);
            
            if (hasShiftToday) {
              canWork = false;
            } else {
              // Check rest period after previous shift
              const previousShift = staff.allAssignments
                .filter(a => !a.isRegularWork && new Date(a.shiftEnd) <= regularWorkStart)
                .sort((a, b) => new Date(b.shiftEnd).getTime() - new Date(a.shiftEnd).getTime())[0];

              if (previousShift) {
                const hoursSinceShiftEnded = (regularWorkStart.getTime() - new Date(previousShift.shiftEnd).getTime()) / (1000 * 60 * 60);
                if (hoursSinceShiftEnded < request.restHoursAfter) {
                  canWork = false;
                }
              }

              // Check rest period before next shift
              const nextShift = staff.allAssignments
                .filter(a => !a.isRegularWork && new Date(a.shiftStart) >= regularWorkEnd)
                .sort((a, b) => new Date(a.shiftStart).getTime() - new Date(b.shiftStart).getTime())[0];

              if (nextShift) {
                const hoursUntilShiftStarts = (new Date(nextShift.shiftStart).getTime() - regularWorkEnd.getTime()) / (1000 * 60 * 60);
                if (hoursUntilShiftStarts < request.restHoursBefore) {
                  canWork = false;
                }
              }
            }

            if (canWork) {
              const assignment: WorkAssignment = {
                staffId: staff.staffId,
                date: date.toISOString(),
                shiftStart: regularWorkStart.toISOString(),
                shiftEnd: regularWorkEnd.toISOString(),
                hours: request.standardHours,
                extendedHours: 0,
                shiftLabel: null,
                shiftNumber: 0,
                isRegularWork: true
              };

              dailySchedule.regularStaffAssignments.push(assignment);
              staff.allAssignments.push(assignment);
              assignedCount++;

              if (assignedCount === 1) {
                regularStaffRotation = (staffIndex + 1) % request.totalStaffCount;
              }
            }
          }

          attempts++;
        }

        totalRegularHours += dailySchedule.regularStaffAssignments.length * request.standardHours;
      }
    }

    // Calculate totals for each staff member
    for (const staff of staffMembers) {
      staff.totalHours = staff.allAssignments.reduce((sum, a) => sum + a.hours, 0);
      staff.totalRegularHours = staff.allAssignments
        .filter(a => a.isRegularWork)
        .reduce((sum, a) => sum + a.hours, 0);
      staff.totalExtendedHours = staff.allAssignments
        .filter(a => !a.isRegularWork)
        .reduce((sum, a) => sum + a.extendedHours, 0);
    }

    const workdays = dailySchedules.filter(d => d.isWorkday).length;

    return {
      totalDays: daysInMonth,
      workdays: workdays,
      nonWorkdays: daysInMonth - workdays,
      totalRegularHours: totalRegularHours,
      totalExtendedHours: totalExtendedHours,
      dailySchedules: dailySchedules,
      staffMembers: staffMembers,
      restViolations: restViolations,
      request: request
    };
  }

  /**
   * Check if staff can work a shift
   * Returns null if they can work, or a RestViolation if they cannot
   */
  private canStaffWorkShift(
    staff: StaffMember,
    shiftStart: Date,
    shiftEnd: Date,
    currentDate: Date,
    request: ScheduleRequest
  ): RestViolation | null {
    // Check monthly extended hours limit
    const currentExtendedHours = staff.allAssignments
      .filter(a => !a.isRegularWork)
      .reduce((sum, a) => sum + a.extendedHours, 0);

    const shiftHoursPerPerson = request.shiftDuration;
    let potentialExtendedHours = 0;

    const isWeekendOrHoliday = !HolidayService.isWorkday(currentDate);

    if (isWeekendOrHoliday) {
      potentialExtendedHours = shiftHoursPerPerson;
    } else {
      if (shiftHoursPerPerson > request.standardHours) {
        potentialExtendedHours = shiftHoursPerPerson - request.standardHours;
      }
    }

    if (currentExtendedHours + potentialExtendedHours > request.maxOnDutyHoursPerMonth) {
      return {
        staffId: staff.staffId,
        date: currentDate.toISOString(),
        previousShiftEnd: new Date(1900, 0, 1).toISOString(), // Use year 1900 to match the check
        nextShiftStart: shiftStart.toISOString(),
        actualRestHours: currentExtendedHours + potentialExtendedHours,
        requiredRestHours: request.maxOnDutyHoursPerMonth,
        reason: `Would exceed monthly extended hours limit (${currentExtendedHours.toFixed(2)}h + ${potentialExtendedHours.toFixed(2)}h = ${(currentExtendedHours + potentialExtendedHours).toFixed(2)}h > ${request.maxOnDutyHoursPerMonth.toFixed(2)}h)`
      };
    }

    // Check rest period after previous shift
    const previousShiftAssignment = staff.allAssignments
      .filter(a => !a.isRegularWork)
      .sort((a, b) => new Date(b.shiftEnd).getTime() - new Date(a.shiftEnd).getTime())[0];

    if (previousShiftAssignment) {
      const hoursSinceLast = (shiftStart.getTime() - new Date(previousShiftAssignment.shiftEnd).getTime()) / (1000 * 60 * 60);

      if (hoursSinceLast < request.restHoursAfter) {
        return {
          staffId: staff.staffId,
          date: currentDate.toISOString(),
          previousShiftEnd: previousShiftAssignment.shiftEnd,
          nextShiftStart: shiftStart.toISOString(),
          actualRestHours: hoursSinceLast,
          requiredRestHours: request.restHoursAfter,
          reason: 'Insufficient rest after previous shift assignment'
        };
      }
    }

    return null;
  }
}
