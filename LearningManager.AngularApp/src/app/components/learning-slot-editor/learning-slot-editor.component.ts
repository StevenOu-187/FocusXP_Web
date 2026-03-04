// @GeneratedCode
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { LearningSlotService } from '../../services/http/learning-slot.service';
import { LearningSlot, LearningSlotEdit } from '../../models/models';

interface DayGroup {
  dayIndex: number;
  label: string;
  slots: LearningSlot[];
  newSlot: LearningSlotEdit;
  adding: boolean;
}

@Component({
  selector: 'app-learning-slot-editor',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './learning-slot-editor.component.html',
  styleUrl: './learning-slot-editor.component.scss'
})
export class LearningSlotEditorComponent implements OnInit {
  days: DayGroup[] = [];
  loading = false;
  error = '';

  private readonly dayNames = ['Montag', 'Dienstag', 'Mittwoch', 'Donnerstag', 'Freitag', 'Samstag', 'Sonntag'];

  constructor(private slotService: LearningSlotService) {}

  ngOnInit(): void {
    this.loadSlots();
  }

  loadSlots(): void {
    this.loading = true;
    this.slotService.getAll().subscribe({
      next: slots => {
        this.days = this.dayNames.map((label, i) => ({
          dayIndex: i,
          label,
          slots: slots.filter(s => s.dayOfWeek === i),
          newSlot: { dayOfWeek: i, startTime: '08:00:00', endTime: '10:00:00' },
          adding: false
        }));
        this.loading = false;
      },
      error: () => { this.error = 'Fehler beim Laden.'; this.loading = false; }
    });
  }

  addSlot(day: DayGroup): void {
    this.error = '';
    const payload: LearningSlotEdit = {
      dayOfWeek: day.dayIndex,
      startTime: day.newSlot.startTime,
      endTime: day.newSlot.endTime
    };
    this.slotService.create(payload).subscribe({
      next: created => {
        day.slots.push(created);
        day.newSlot = { dayOfWeek: day.dayIndex, startTime: '08:00:00', endTime: '10:00:00' };
        day.adding = false;
      },
      error: err => {
        this.error = err?.error?.message ?? 'Zeitblock konnte nicht gespeichert werden.';
      }
    });
  }

  deleteSlot(day: DayGroup, slot: LearningSlot): void {
    this.slotService.delete(slot.id).subscribe({
      next: () => { day.slots = day.slots.filter(s => s.id !== slot.id); }
    });
  }

  formatTime(t: string): string {
    return t.substring(0, 5);
  }
}
