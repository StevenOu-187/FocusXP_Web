// @GeneratedCode
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { TaskItemService } from '../../services/http/task-item.service';
import { TaskItemEdit } from '../../models/models';

@Component({
  selector: 'app-task-form',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './task-form.component.html',
  styleUrl: './task-form.component.scss'
})
export class TaskFormComponent implements OnInit {
  editId: number | null = null;
  saving = false;
  error = '';

  model: TaskItemEdit = {
    title: '',
    description: '',
    dueDate: new Date().toISOString().substring(0, 10),
    estimatedHours: 1,
    status: 'Open'
  };

  readonly statusOptions = ['Open', 'InProgress', 'Done'];

  constructor(
    private taskService: TaskItemService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.editId = +id;
      this.taskService.getById(this.editId).subscribe({
        next: t => {
          this.model = {
            title: t.title,
            description: t.description,
            dueDate: t.dueDate.substring(0, 10),
            estimatedHours: t.estimatedHours,
            status: t.status
          };
        }
      });
    }
  }

  save(): void {
    this.saving = true;
    const op = this.editId
      ? this.taskService.update(this.editId, this.model)
      : this.taskService.create(this.model);

    op.subscribe({
      next: () => this.router.navigate(['/tasks']),
      error: () => { this.error = 'Speichern fehlgeschlagen.'; this.saving = false; }
    });
  }

  cancel(): void {
    this.router.navigate(['/tasks']);
  }
}
