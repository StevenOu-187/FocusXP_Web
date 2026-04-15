// @GeneratedCode
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ScheduleService } from '../../services/http/schedule.service';
import { TaskItemService } from '../../services/http/task-item.service';
import { BlockedDayService } from '../../services/http/blocked-day.service';
import { BlockedDay, ScheduleEntry, TaskItem } from '../../models/models';

interface CalDay {
  date: Date;
  dateNum: number;
  currentMonth: boolean;
  isToday: boolean;
  entries: ScheduleEntry[];
  isBlocked: boolean;
  blockedDay: BlockedDay | null;
}

@Component({
  selector: 'app-calendar',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './calendar.component.html',
  styleUrl: './calendar.component.scss'
})
export class CalendarComponent implements OnInit {
  viewDate = new Date();
  weeks: CalDay[][] = [];
  selectedDay: CalDay | null = null;
  dayViewOpen = false;
  loading = false;
  error = '';

  blockModalOpen = false;
  blockReason = '';
  blockSaving = false;

  private allEntries: ScheduleEntry[] = [];
  private taskStatusById = new Map<number, TaskItem['status']>();
  private blockedDays: BlockedDay[] = [];

  readonly weekdays = ['MO', 'DI', 'MI', 'DO', 'FR', 'SA', 'SO'];
  readonly monthNames = [
    'Januar','Februar','März','April','Mai','Juni',
    'Juli','August','September','Oktober','November','Dezember'
  ];

  constructor(
    private scheduleService: ScheduleService,
    private taskService: TaskItemService,
    private blockedDayService: BlockedDayService
  ) {}

  ngOnInit(): void { this.load(); }

  get monthLabel(): string {
    return `${this.monthNames[this.viewDate.getMonth()]} ${this.viewDate.getFullYear()}`;
  }

  prevMonth(): void {
    this.viewDate = new Date(this.viewDate.getFullYear(), this.viewDate.getMonth() - 1, 1);
    this.load();
  }

  nextMonth(): void {
    this.viewDate = new Date(this.viewDate.getFullYear(), this.viewDate.getMonth() + 1, 1);
    this.load();
  }

  goToday(): void {
    this.viewDate = new Date();
    this.load();
  }

  load(): void {
    this.loading = true;
    this.error = '';

    // Determine the full visible date range of the calendar grid
    const firstOfMonth = new Date(this.viewDate.getFullYear(), this.viewDate.getMonth(), 1);
    const rangeFrom    = this.getMondayOf(firstOfMonth);           // first Monday shown
    const rangeTo      = new Date(rangeFrom);
    rangeTo.setDate(rangeTo.getDate() + 6 * 7 - 1);               // at most 6 weeks visible

    this.blockedDayService.getAll().subscribe({
      next: blocked => {
        this.blockedDays = blocked;
        this.scheduleService.getScheduleRange(rangeFrom, rangeTo).subscribe({
          next: entries => {
            this.allEntries = entries;
            this.taskService.getAll().subscribe({
              next: tasks => {
                this.taskStatusById = new Map(tasks.map(t => [t.id, t.status] as const));
                this.buildGrid();
                this.loading = false;
              },
              error: () => {
                this.taskStatusById = new Map();
                this.buildGrid();
                this.loading = false;
              }
            });
          },
          error: () => {
            this.allEntries = [];
            this.taskStatusById = new Map();
            this.buildGrid();
            this.loading = false;
          }
        });
      },
      error: () => {
        this.blockedDays = [];
        this.loading = false;
      }
    });
  }

  private buildGrid(): void {
    const today = new Date();
    const year  = this.viewDate.getFullYear();
    const month = this.viewDate.getMonth();

    // Start from the Monday of the week containing the 1st
    const firstOfMonth = new Date(year, month, 1);
    let cursor = this.getMondayOf(firstOfMonth);

    this.weeks = [];
    for (let w = 0; w < 6; w++) {
      const week: CalDay[] = [];
      for (let d = 0; d < 7; d++) {
        const day = new Date(cursor);
        const entriesForDay = this.allEntries.filter(e => {
          const eDate = new Date(e.date);
          return eDate.getFullYear() === day.getFullYear() &&
                 eDate.getMonth()    === day.getMonth() &&
                 eDate.getDate()     === day.getDate();
        });
        const dayIso = this.toIsoDate(day);
        const blocked = this.blockedDays.find(b => b.date === dayIso) ?? null;
        week.push({
          date: day,
          dateNum: day.getDate(),
          currentMonth: day.getMonth() === month,
          isToday: this.sameDay(day, today),
          entries: entriesForDay,
          isBlocked: blocked !== null,
          blockedDay: blocked
        });
        cursor.setDate(cursor.getDate() + 1);
      }
      this.weeks.push(week);
      // Stop if the week is entirely in the next month
      if (week.every(d => !d.currentMonth && w >= 3)) break;
    }
  }

  private getMondayOf(date: Date): Date {
    const d = new Date(date);
    const day = d.getDay();
    const diff = day === 0 ? -6 : 1 - day;
    d.setDate(d.getDate() + diff);
    d.setHours(0, 0, 0, 0);
    return d;
  }

  private sameDay(a: Date, b: Date): boolean {
    return a.getFullYear() === b.getFullYear() &&
           a.getMonth()    === b.getMonth() &&
           a.getDate()     === b.getDate();
  }

  toIsoDate(d: Date): string {
    const y = d.getFullYear();
    const m = String(d.getMonth() + 1).padStart(2, '0');
    const day = String(d.getDate()).padStart(2, '0');
    return `${y}-${m}-${day}`;
  }

  formatTime(t: string): string { return t.substring(0, 5); }

  openDayView(day: CalDay): void {
    this.selectedDay = day;
    this.dayViewOpen = true;
    this.blockModalOpen = false;
    this.blockReason = '';
  }

  closeDayView(): void {
    this.dayViewOpen = false;
  }

  openBlockModal(): void {
    this.blockReason = this.selectedDay?.blockedDay?.reason ?? '';
    this.blockModalOpen = true;
  }

  closeBlockModal(): void {
    this.blockModalOpen = false;
  }

  blockDay(): void {
    if (!this.selectedDay) return;
    this.blockSaving = true;
    const dateStr = this.toIsoDate(this.selectedDay.date);
    this.blockedDayService.create({ date: dateStr, reason: this.blockReason || null }).subscribe({
      next: () => {
        this.blockSaving = false;
        this.blockModalOpen = false;
        this.dayViewOpen = false;
        this.load();
      },
      error: () => { this.blockSaving = false; }
    });
  }

  unblockDay(): void {
    if (!this.selectedDay?.blockedDay) return;
    this.blockSaving = true;
    this.blockedDayService.delete(this.selectedDay.blockedDay.id).subscribe({
      next: () => {
        this.blockSaving = false;
        this.dayViewOpen = false;
        this.load();
      },
      error: () => { this.blockSaving = false; }
    });
  }

  get selectedDayTasks(): Array<{ entry: ScheduleEntry; status: TaskItem['status'] }> {
    if (!this.selectedDay) return [];
    return [...this.selectedDay.entries]
      .sort((a, b) => a.startTime.localeCompare(b.startTime))
      .map(entry => ({
        entry,
        status: this.taskStatusById.get(entry.taskItemId) ?? 'Open'
      }));
  }

  get openCount(): number {
    return this.selectedDayTasks.filter(t => t.status !== 'Done').length;
  }

  get doneCount(): number {
    return this.selectedDayTasks.filter(t => t.status === 'Done').length;
  }

  get selectedDayTitle(): string {
    if (!this.selectedDay) return '';
    const today = new Date();
    if (this.sameDay(this.selectedDay.date, today)) return 'Heute';
    const tomorrow = new Date(today);
    tomorrow.setDate(tomorrow.getDate() + 1);
    if (this.sameDay(this.selectedDay.date, tomorrow)) return 'Morgen';
    return `${this.selectedDay.date.getDate()}. ${this.monthNames[this.selectedDay.date.getMonth()]}`;
  }

  statusLabel(status: TaskItem['status']): string {
    switch (status) {
      case 'Done': return 'Erledigt';
      case 'InProgress': return 'In Arbeit';
      default: return 'Offen';
    }
  }

  get flatDays(): CalDay[] { return this.weeks.flat(); }
}
