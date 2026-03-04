// @GeneratedCode
import { Routes } from '@angular/router';
import { CalendarComponent } from './components/calendar/calendar.component';
import { TaskListComponent } from './components/task-list/task-list.component';
import { TaskFormComponent } from './components/task-form/task-form.component';
import { LearningSlotEditorComponent } from './components/learning-slot-editor/learning-slot-editor.component';
import { ProfileComponent } from './components/profile/profile.component';

export const routes: Routes = [
  { path: '',          redirectTo: 'calendar', pathMatch: 'full' },
  { path: 'calendar',  component: CalendarComponent },
  { path: 'tasks',     component: TaskListComponent },
  { path: 'tasks/new', component: TaskFormComponent },
  { path: 'tasks/edit/:id', component: TaskFormComponent },
  { path: 'slots',     component: LearningSlotEditorComponent },
  { path: 'profile',   component: ProfileComponent },
];
