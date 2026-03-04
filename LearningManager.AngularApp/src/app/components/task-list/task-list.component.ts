// @GeneratedCode
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { TaskItemService } from '../../services/http/task-item.service';
import { TaskItem } from '../../models/models';

@Component({
  selector: 'app-task-list',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './task-list.component.html',
  styleUrl: './task-list.component.scss'
})
export class TaskListComponent implements OnInit {
  tasks: TaskItem[] = [];
  loading = false;
  error = '';

  constructor(private taskService: TaskItemService) {}

  ngOnInit(): void {
    this.loadTasks();
  }

  loadTasks(): void {
    this.loading = true;
    this.taskService.getAll().subscribe({
      next: tasks => { this.tasks = tasks; this.loading = false; },
      error: () => { this.error = 'Fehler beim Laden der Aufgaben.'; this.loading = false; }
    });
  }

  markDone(task: TaskItem): void {
    this.taskService.patchStatus(task.id, 'Done').subscribe({
      next: updated => {
        const idx = this.tasks.findIndex(t => t.id === updated.id);
        if (idx !== -1) this.tasks[idx] = updated;
      }
    });
  }

  delete(task: TaskItem): void {
    if (!confirm(`Aufgabe "${task.title}" wirklich löschen?`)) return;
    this.taskService.delete(task.id).subscribe({
      next: () => { this.tasks = this.tasks.filter(t => t.id !== task.id); }
    });
  }

  statusBadgeClass(status: string): string {
    switch (status) {
      case 'Done':       return 'bg-success';
      case 'InProgress': return 'bg-warning text-dark';
      default:           return 'bg-secondary';
    }
  }

  statusLabel(status: string): string {
    switch (status) {
      case 'Done':       return 'Erledigt';
      case 'InProgress': return 'In Bearbeitung';
      default:           return 'Offen';
    }
  }
}
