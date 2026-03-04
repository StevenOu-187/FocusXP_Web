import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.scss'
})
export class ProfileComponent {
  readonly user = {
    initials: 'MA',
    username: 'max_mueller',
    handle: '@max_mueller',
    mail: 'max@example.com',
    firstName: 'Max',
    lastName: 'Mueller'
  };
}
