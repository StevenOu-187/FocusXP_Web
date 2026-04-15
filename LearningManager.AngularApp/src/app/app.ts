// @GeneratedCode
import { CommonModule } from '@angular/common';
import { Component, HostListener, OnInit } from '@angular/core';
import { RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, RouterOutlet, RouterLink, RouterLinkActive],
  template: `
    <header class="lm-navbar">
      <a class="lm-brand" [routerLink]="['/calendar']">FocusXP</a>
      <nav class="lm-nav-links">
        <a [routerLink]="['/calendar']"     routerLinkActive="active">📅 Kalender</a>
        <a [routerLink]="['/tasks']"        routerLinkActive="active">📋 Aufgaben</a>
        <a [routerLink]="['/slots']"        routerLinkActive="active">📊 Lernraster</a>
        <a [routerLink]="['/blocked-days']" routerLinkActive="active">🚫 Gesperrt</a>
      </nav>
      <div class="lm-nav-right">
        <button class="lm-dark-toggle" (click)="toggleDark()" [title]="isDark ? 'Light Mode' : 'Dark Mode'">
          {{ isDark ? '☀️' : '🌙' }}
        </button>

        <div class="lm-user-menu" [class.open]="menuOpen">
          <button class="lm-avatar-btn" (click)="toggleMenu($event)" aria-label="Benutzermenü öffnen">
            <div class="lm-avatar">MA</div>
          </button>

          <div *ngIf="menuOpen" class="lm-user-popover">
            <div class="lm-user-head">
              <div class="lm-user-avatar-lg">MA</div>
              <div class="lm-user-meta">
                <div class="lm-user-name">max_mueller</div>
                <div class="lm-user-mail">max@example.com</div>
              </div>
            </div>

            <div class="lm-user-actions">
              <a [routerLink]="['/profile']" (click)="closeMenu()">Profil</a>
              <button type="button" class="danger" (click)="logout()">Logout</button>
            </div>
          </div>
        </div>
      </div>
    </header>
    <router-outlet />
  `,
  styleUrl: './app.scss'
})
export class App implements OnInit {
  readonly title = 'FocusXP';
  isDark = false;
  menuOpen = false;

  ngOnInit(): void {
    const saved = localStorage.getItem('lm-theme');
    this.isDark = saved === 'dark' || (!saved && window.matchMedia('(prefers-color-scheme: dark)').matches);
    this.applyTheme();
  }

  toggleDark(): void {
    this.isDark = !this.isDark;
    localStorage.setItem('lm-theme', this.isDark ? 'dark' : 'light');
    this.applyTheme();
  }

  toggleMenu(event: MouseEvent): void {
    event.stopPropagation();
    this.menuOpen = !this.menuOpen;
  }

  closeMenu(): void {
    this.menuOpen = false;
  }

  logout(): void {
    this.menuOpen = false;
    alert('Logout ausgelöst (Demo).');
  }

  @HostListener('document:click')
  onDocumentClick(): void {
    this.menuOpen = false;
  }

  private applyTheme(): void {
    document.documentElement.setAttribute('data-theme', this.isDark ? 'dark' : 'light');
  }
}

