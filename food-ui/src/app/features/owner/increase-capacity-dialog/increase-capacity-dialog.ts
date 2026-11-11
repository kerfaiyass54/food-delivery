import { Component, computed, input, output, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({ selector:'app-increase-capacity-dialog', standalone:true, imports:[CommonModule,FormsModule], templateUrl:'./increase-capacity-dialog.html', styleUrl:'./increase-capacity-dialog.css' })
export class IncreaseCapacityDialogComponent {
  currentCapacity = input.required<number>();
  costPerSeat = input(125);
  requested = output<{ capacity: number; cost: number }>();
  cancelled = output<void>();
  additionalSeats = signal(8);
  totalCost = computed(() => Math.max(0, this.additionalSeats()) * this.costPerSeat());
  setSeats(value: string): void { this.additionalSeats.set(Math.max(1, Number(value) || 1)); }
}
