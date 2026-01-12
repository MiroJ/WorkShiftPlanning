import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { ScheduleRequest, ScheduleResult } from '../models/schedule.models';
import { ScheduleCalculatorService } from './schedule-calculator.service';

@Injectable({
  providedIn: 'root'
})
export class ScheduleService {
  constructor(private calculatorService: ScheduleCalculatorService) { }

  calculateSchedule(request: ScheduleRequest): Observable<ScheduleResult> {
    // Calculate schedule locally in the browser
    const result = this.calculatorService.calculateSchedule(request);
    return of(result);
  }
}
