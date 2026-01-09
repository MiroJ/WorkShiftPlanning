import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { ScheduleResult } from '../models/schedule.models';

@Injectable({
  providedIn: 'root'
})
export class ScheduleDataService {
  private scheduleResultSubject = new BehaviorSubject<ScheduleResult | null>(null);
  public scheduleResult$: Observable<ScheduleResult | null> = this.scheduleResultSubject.asObservable();

  setScheduleResult(result: ScheduleResult): void {
    this.scheduleResultSubject.next(result);
  }

  getScheduleResult(): ScheduleResult | null {
    return this.scheduleResultSubject.value;
  }

  clearScheduleResult(): void {
    this.scheduleResultSubject.next(null);
  }
}
