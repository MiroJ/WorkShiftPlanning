import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ScheduleRequest, ScheduleResult } from '../models/schedule.models';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ScheduleService {
  private apiUrl = `${environment.apiUrl}/schedule`;

  constructor(private http: HttpClient) { }

  calculateSchedule(request: ScheduleRequest): Observable<ScheduleResult> {
    return this.http.post<ScheduleResult>(`${this.apiUrl}/calculate`, request);
  }
}
