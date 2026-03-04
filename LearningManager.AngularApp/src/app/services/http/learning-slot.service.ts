// @GeneratedCode
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { LearningSlot, LearningSlotEdit } from '../../models/models';

@Injectable({ providedIn: 'root' })
export class LearningSlotService {
  private readonly base = '/api/learningslot';

  constructor(private http: HttpClient) {}

  getAll(): Observable<LearningSlot[]> {
    return this.http.get<LearningSlot[]>(this.base);
  }

  getById(id: number): Observable<LearningSlot> {
    return this.http.get<LearningSlot>(`${this.base}/${id}`);
  }

  create(slot: LearningSlotEdit): Observable<LearningSlot> {
    return this.http.post<LearningSlot>(this.base, slot);
  }

  update(id: number, slot: LearningSlotEdit): Observable<LearningSlot> {
    return this.http.put<LearningSlot>(`${this.base}/${id}`, slot);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.base}/${id}`);
  }
}
