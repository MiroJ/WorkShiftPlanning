import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { ScheduleService } from '../../services/schedule.service';
import { ScheduleDataService } from '../../services/schedule-data.service';
import { ScheduleRequest } from '../../models/schedule.models';

@Component({
    selector: 'app-schedule-input',
    templateUrl: './schedule-input.component.html',
    styleUrls: ['./schedule-input.component.css'],
    standalone: false
})
export class ScheduleInputComponent {
  isLoading = false;
  errorMessage = '';

  scheduleRequest: ScheduleRequest = {
    year: new Date().getFullYear(),
    month: 3,
    totalStaffCount: 12,
    numberOfShifts: 2,
    staffPerShift: 1,
    firstShiftStartHour: 8,
    regularStartHour: 8,
    shiftDuration: 12,
    standardHours: 7.5,
    maxOnDutyHoursPerMonth: 36,
    restHoursBefore: 12,
    restHoursAfter: 24
  };

  constructor(
    private scheduleService: ScheduleService,
    private scheduleDataService: ScheduleDataService,
    private router: Router
  ) {
    this.calculateShiftDuration();
  }

  onNumberOfShiftsChange(): void {
    this.calculateShiftDuration();
  }

  calculateShiftDuration(): void {
    if (this.scheduleRequest.numberOfShifts > 0) {
      this.scheduleRequest.shiftDuration = 24 / this.scheduleRequest.numberOfShifts;
    }
  }

  getWorkdaysInMonth(): number {
    const daysInMonth = new Date(this.scheduleRequest.year, this.scheduleRequest.month, 0).getDate();
    let workdays = 0;
    
    for (let day = 1; day <= daysInMonth; day++) {
      const date = new Date(this.scheduleRequest.year, this.scheduleRequest.month - 1, day);
      const dayOfWeek = date.getDay();
      if (dayOfWeek !== 0 && dayOfWeek !== 6) {
        workdays++;
      }
    }
    
    return workdays;
  }

  getMaxWorkHours(): number {
    const workdays = this.getWorkdaysInMonth();
    return (workdays * this.scheduleRequest.standardHours) + this.scheduleRequest.maxOnDutyHoursPerMonth;
  }

  getMonthName(): string {
    const date = new Date(this.scheduleRequest.year, this.scheduleRequest.month - 1, 1);
    return date.toLocaleString('default', { month: 'long', year: 'numeric' });
  }

  onSubmit(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.scheduleService.calculateSchedule(this.scheduleRequest).subscribe({
      next: (result) => {
        this.isLoading = false;
        // Store result in the shared service
        this.scheduleDataService.setScheduleResult(result);
        // Navigate to results page
        this.router.navigate(['/results']);
      },
      error: (error) => {
        this.isLoading = false;
        this.errorMessage = error.error?.error || 'An error occurred while calculating the schedule.';
        console.error('Error calculating schedule:', error);
      }
    });
  }
}
