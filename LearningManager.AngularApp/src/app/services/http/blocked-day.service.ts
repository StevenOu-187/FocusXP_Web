import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { BlockedDay, BlockedDayEdit } from '../../models/models';

@Injectable({ providedIn: 'root' })
export class BlockedDayService {
  private readonly base = '/api/blockedday';

  constructor(private http: HttpClient) {}

  getAll(): Observable<BlockedDay[]> {
    return this.http.get<BlockedDay[]>(this.base);
  }

  create(day: BlockedDayEdit): Observable<BlockedDay> {
    return this.http.post<BlockedDay>(this.base, day);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.base}/${id}`);
  }

  deleteByDate(date: string): Observable<void> {
    return this.http.delete<void>(`${this.base}/by-date/${date}`);
  }
}
