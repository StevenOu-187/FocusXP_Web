// @GeneratedCode
export interface LearningSlot {
  id: number;
  dayOfWeek: number;
  startTime: string;
  endTime: string;
}

export interface LearningSlotEdit {
  dayOfWeek: number;
  startTime: string;
  endTime: string;
}

export interface TaskItem {
  id: number;
  title: string;
  description: string;
  dueDate: string;
  estimatedHours: number;
  status: 'Open' | 'InProgress' | 'Done';
}

export interface TaskItemEdit {
  title: string;
  description: string;
  dueDate: string;
  estimatedHours: number;
  status: string;
}

export interface ScheduleEntry {
  date: string;
  startTime: string;
  endTime: string;
  taskTitle: string;
  taskItemId: number;
}
