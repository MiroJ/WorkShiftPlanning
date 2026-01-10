import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ScheduleResult, DailySchedule, StaffMember } from '../../models/schedule.models';
import { ScheduleDataService } from '../../services/schedule-data.service';

@Component({
    selector: 'app-schedule-results',
    templateUrl: './schedule-results.component.html',
    styleUrls: ['./schedule-results.component.css'],
    standalone: false
})
export class ScheduleResultsComponent implements OnInit {
  scheduleResult: ScheduleResult | null = null;
  selectedView: 'summary' | 'daily' | 'staff' | 'violations' = 'summary';
  selectedStaffId: string | null = null;

  // Expose Object.keys to the template
  Object = Object;

  constructor(
    private router: Router,
    private scheduleDataService: ScheduleDataService
  ) { }

  ngOnInit(): void {
    // Scroll to top of page when component loads
    window.scrollTo(0, 0);
    
    // Get the schedule result from the service
    this.scheduleResult = this.scheduleDataService.getScheduleResult();
    
    // If no result, redirect back to input page
    if (!this.scheduleResult) {
      console.error('No schedule result found, redirecting to input page');
      this.router.navigate(['/']);
    }
  }

  goBack(): void {
    this.scheduleDataService.clearScheduleResult();
    this.router.navigate(['/']);
  }

  getShiftLabels(): string[] {
    if (!this.scheduleResult) return [];
    const labels: string[] = [];
    for (let i = 0; i < this.scheduleResult.request.numberOfShifts; i++) {
      const startHour = (this.scheduleResult.request.firstShiftStartHour + 
                        (i * this.scheduleResult.request.shiftDuration)) % 24;
      const endHour = (this.scheduleResult.request.firstShiftStartHour + 
                      ((i + 1) * this.scheduleResult.request.shiftDuration)) % 24;
      labels.push(`Shift ${String.fromCharCode(65 + i)}: ${this.formatHour(startHour)} - ${this.formatHour(endHour)}`);
    }
    return labels;
  }

  formatHour(hour: number): string {
    return `${Math.floor(hour).toString().padStart(2, '0')}:00`;
  }

  formatTime(dateString: string): string {
    const date = new Date(dateString);
    return date.toLocaleTimeString('en-US', { hour: '2-digit', minute: '2-digit', hour12: false });
  }

  formatDate(dateString: string): string {
    const date = new Date(dateString);
    return date.toLocaleDateString('en-US', { weekday: 'short', year: 'numeric', month: 'short', day: 'numeric' });
  }

  getDayOfWeek(dateString: string): string {
    const date = new Date(dateString);
    return date.toLocaleDateString('en-US', { weekday: 'short' });
  }

  getStaffShiftDistribution(staff: StaffMember): { [key: string]: number } {
    const distribution: { [key: string]: number } = {};
    staff.allAssignments
      .filter(a => !a.isRegularWork && a.shiftLabel)
      .forEach(a => {
        const label = a.shiftLabel!;
        distribution[label] = (distribution[label] || 0) + 1;
      });
    return distribution;
  }

  selectStaff(staffId: string): void {
    this.selectedStaffId = this.selectedStaffId === staffId ? null : staffId;
  }

  getWorkloadStats() {
    if (!this.scheduleResult) return null;
    
    const hours = this.scheduleResult.staffMembers.map(s => s.totalHours);
    const min = Math.min(...hours);
    const max = Math.max(...hours);
    const avg = hours.reduce((a, b) => a + b, 0) / hours.length;
    const avgAssignments = this.scheduleResult.staffMembers
      .reduce((sum, s) => sum + s.allAssignments.length, 0) / this.scheduleResult.staffMembers.length;

    return { min, max, avg, variance: max - min, avgAssignments };
  }

  getViolationCountForStaff(staffId: string): number {
    if (!this.scheduleResult) return 0;
    return this.scheduleResult.restViolations.filter(v => v.staffId === staffId).length;
  }

  isOverExtendedHoursLimit(staff: StaffMember): boolean {
    return staff.totalExtendedHours > this.scheduleResult!.request.maxOnDutyHoursPerMonth;
  }

  getSortedAssignments(staff: StaffMember): any[] {
    return staff.allAssignments.slice().sort((a, b) => {
      const dateA = new Date(a.date).getTime();
      const dateB = new Date(b.date).getTime();
      if (dateA !== dateB) {
        return dateA - dateB;
      }
      // If same date, sort by shift start time
      return new Date(a.shiftStart).getTime() - new Date(b.shiftStart).getTime();
    });
  }

  isMonthlyLimitViolation(violation: any): boolean {
    // Monthly limit violations have DateTime.MinValue for PreviousShiftEnd (year 1)
    const previousShiftDate = new Date(violation.previousShiftEnd);
    return previousShiftDate.getFullYear() <= 1900;
  }
}
