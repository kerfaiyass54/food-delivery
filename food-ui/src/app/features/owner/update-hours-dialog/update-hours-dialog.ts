import { Component, input, output, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';

@Component({ selector:'app-update-hours-dialog', standalone:true, imports:[FormsModule], templateUrl:'./update-hours-dialog.html', styleUrl:'./update-hours-dialog.css' })
export class UpdateHoursDialogComponent {
  opening = input.required<string>(); closing = input.required<string>();
  requested = output<{ timeOpen: string; timeClose: string }>(); cancelled = output<void>();
  timeOpen = signal(''); timeClose = signal('');
  ngOnInit(): void { this.timeOpen.set(this.opening()); this.timeClose.set(this.closing()); }
}
