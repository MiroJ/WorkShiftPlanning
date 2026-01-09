export interface ScheduleRequest {
  year: number;
  month: number;
  totalStaffCount: number;
  numberOfShifts: number;
  staffPerShift: number;
  firstShiftStartHour: number;
  regularStartHour: number;
  shiftDuration: number;
  standardHours: number;
  maxOnDutyHoursPerMonth: number;
  restHoursBefore: number;
  restHoursAfter: number;
}

export interface ScheduleResult {
  totalDays: number;
  workdays: number;
  nonWorkdays: number;
  totalRegularHours: number;
  totalExtendedHours: number;
  dailySchedules: DailySchedule[];
  staffMembers: StaffMember[];
  restViolations: RestViolation[];
  request: ScheduleRequest;
}

export interface DailySchedule {
  date: string;
  isWorkday: boolean;
  regularStaffNeeded: number;
  shiftStaffNeeded: number;
  regularHours: number;
  extendedHours: number;
  regularStaffAssignments: WorkAssignment[];
  shiftAssignments: WorkAssignment[];
}

export interface StaffMember {
  staffId: string;
  totalHours: number;
  totalRegularHours: number;
  totalExtendedHours: number;
  allAssignments: WorkAssignment[];
}

export interface WorkAssignment {
  staffId: string;
  date: string;
  shiftStart: string;
  shiftEnd: string;
  hours: number;
  extendedHours: number;
  shiftLabel: string | null;
  shiftNumber: number;
  isRegularWork: boolean;
}

export interface RestViolation {
  staffId: string;
  date: string;
  previousShiftEnd: string;
  nextShiftStart: string;
  actualRestHours: number;
  requiredRestHours: number;
  reason: string;
}
