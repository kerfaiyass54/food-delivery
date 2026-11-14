import { Component, input, output, signal } from '@angular/core';

@Component({
  selector: 'app-update-days-dialog',
  standalone: true,
  templateUrl: './update-days-dialog.html',
  styleUrl: './update-days-dialog.css',
})
export class UpdateDaysDialogComponent {
  days = input.required<string[]>();
  offDays = input.required<string[]>();
  requested = output<{ days: string[]; offDays: string[] }>();
  cancelled = output<void>();
  readonly weekdays = [
    'Monday',
    'Tuesday',
    'Wednesday',
    'Thursday',
    'Friday',
    'Saturday',
    'Sunday',
  ];
  selected = signal<string[]>([]);
  ngOnInit(): void {
    this.selected.set(this.days());
  }
  toggle(day: string): void {
    this.selected.update(value =>
      value.includes(day)
        ? value.filter(item => item !== day)
        : this.weekdays.filter(item => item === day || value.includes(item))
    );
  }
  isSelected(day: string): boolean {
    return this.selected().includes(day);
  }
  submit(): void {
    const days = this.weekdays.filter(day => this.selected().includes(day));
    this.requested.emit({ days, offDays: this.weekdays.filter(day => !days.includes(day)) });
  }
}
