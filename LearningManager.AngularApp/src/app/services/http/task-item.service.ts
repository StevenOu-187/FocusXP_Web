// @GeneratedCode
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { TaskItem, TaskItemEdit } from '../../models/models';

@Injectable({ providedIn: 'root' })
export class TaskItemService {
  private readonly base = '/api/taskitem';

  constructor(private http: HttpClient) {}

  getAll(): Observable<TaskItem[]> {
    return this.http.get<TaskItem[]>(this.base);
  }

  getById(id: number): Observable<TaskItem> {
    return this.http.get<TaskItem>(`${this.base}/${id}`);
  }

  create(item: TaskItemEdit): Observable<TaskItem> {
    return this.http.post<TaskItem>(this.base, item);
  }

  update(id: number, item: TaskItemEdit): Observable<TaskItem> {
    return this.http.put<TaskItem>(`${this.base}/${id}`, item);
  }

  patchStatus(id: number, status: string): Observable<TaskItem> {
    return this.http.patch<TaskItem>(`${this.base}/${id}/status`, { status });
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.base}/${id}`);
  }
}
