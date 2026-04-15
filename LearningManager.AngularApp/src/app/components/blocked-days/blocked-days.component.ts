import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { BlockedDayService } from '../../services/http/blocked-day.service';
import { BlockedDay } from '../../models/models';

@Component({
  selector: 'app-blocked-days',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './blocked-days.component.html'
})
export class BlockedDaysComponent implements OnInit {
  blockedDays: BlockedDay[] = [];
  loading = false;
  error = '';

  // Form state
  adding = false;
  newDate = '';
  newReason = '';
  saving = false;
  formError = '';

  constructor(private service: BlockedDayService) {}

  ngOnInit(): void { this.load(); }

  load(): void {
    this.loading = true;
    this.service.getAll().subscribe({
      next: days => { this.blockedDays = days; this.loading = false; },
      error: () => { this.error = 'Fehler beim Laden.'; this.loading = false; }
    });
  }

  startAdding(): void {
    this.adding = true;
    this.newDate = '';
    this.newReason = '';
    this.formError = '';
  }

  cancelAdding(): void {
    this.adding = false;
  }

  save(): void {
    if (!this.newDate) { this.formError = 'Bitte ein Datum angeben.'; return; }
    this.saving = true;
    this.formError = '';
    this.service.create({ date: this.newDate, reason: this.newReason || null }).subscribe({
      next: () => {
        this.saving = false;
        this.adding = false;
        this.load();
      },
      error: () => { this.saving = false; this.formError = 'Fehler beim Speichern.'; }
    });
  }

  delete(day: BlockedDay): void {
    this.service.delete(day.id).subscribe({ next: () => this.load() });
  }

  formatDate(iso: string): string {
    const d = new Date(iso);
    return d.toLocaleDateString('de-AT', { day: '2-digit', month: '2-digit', year: 'numeric' });
  }
}
