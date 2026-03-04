// @GeneratedCode
import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ScheduleEntry } from '../../models/models';

@Injectable({ providedIn: 'root' })
export class ScheduleService {
  private readonly base = '/api/schedule';

  constructor(private http: HttpClient) {}

  /** Load schedule for an arbitrary date range */
  getScheduleRange(from: Date, to: Date): Observable<ScheduleEntry[]> {
    const params = new HttpParams()
      .set('from', from.toISOString().substring(0, 10))
      .set('to',   to.toISOString().substring(0, 10));
    return this.http.get<ScheduleEntry[]>(this.base, { params });
  }

  /** Convenience: single week */
  getSchedule(weekStart: Date): Observable<ScheduleEntry[]> {
    const to = new Date(weekStart);
    to.setDate(to.getDate() + 6);
    return this.getScheduleRange(weekStart, to);
  }
}
