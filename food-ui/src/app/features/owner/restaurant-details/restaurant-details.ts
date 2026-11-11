import { Component, inject, signal } from '@angular/core';
import { DatePipe, NgClass } from '@angular/common';
import { OwnerRestaurantService } from '../../../core/services/owner/owner-restaurant.service';
import { RestaurantService } from '../../../core/services/restaurant.service';
import { ConfirmOperationDialogComponent } from '../confirm-operation-dialog/confirm-operation-dialog';
import { IncreaseCapacityDialogComponent } from '../increase-capacity-dialog/increase-capacity-dialog';
import { UpdateDaysDialogComponent } from '../update-days-dialog/update-days-dialog';
import { UpdateHoursDialogComponent } from '../update-hours-dialog/update-hours-dialog';
import { RestaurantDetails } from '../../../shared/models/owner.models';

type DialogKind = 'capacity' | 'hours' | 'days';
interface PendingOperation { kind: DialogKind; title: string; message: string; changes: Partial<RestaurantDetails>; }

@Component({ selector:'app-restaurant-details', standalone:true, imports:[DatePipe,NgClass,IncreaseCapacityDialogComponent,UpdateHoursDialogComponent,UpdateDaysDialogComponent,ConfirmOperationDialogComponent], templateUrl:'./restaurant-details.html' })
export class RestaurantDetailsComponent {
  readonly data = inject(OwnerRestaurantService);
  readonly restaurantService = inject(RestaurantService);
  readonly activeDialog = signal<DialogKind | null>(null);
  readonly pendingOperation = signal<PendingOperation | null>(null);

  stageCapacity(event: {capacity:number;cost:number}): void {
    this.activeDialog.set(null);
    this.pendingOperation.set({kind:'capacity',title:'Increase restaurant capacity?',message:`Your capacity will increase to ${event.capacity} guests. Estimated one-time cost: $${event.cost.toLocaleString()}.`,changes:{capacity:event.capacity}});
  }
  stageHours(event: {timeOpen:string;timeClose:string}): void {
    this.activeDialog.set(null);
    this.pendingOperation.set({kind:'hours',title:'Update opening hours?',message:`Your daily service hours will change to ${event.timeOpen}–${event.timeClose}.`,changes:event});
  }
  stageDays(event: {days:string[];offDays:string[]}): void {
    this.activeDialog.set(null);
    this.pendingOperation.set({kind:'days',title:'Update your service days?',message:`You’ll be open ${event.days.length} days a week, with ${event.offDays.length} day${event.offDays.length === 1 ? '' : 's'} off.`,changes:event});
  }
  async confirmOperation(): Promise<void> {
    const pending = this.pendingOperation();
    if (!pending) return;

    try {
      if (pending.kind === 'capacity') {
        await this.restaurantService.updateCapacity(pending.changes.capacity!);
      } else if (pending.kind === 'hours') {
        await this.restaurantService.updateOpeningHours(pending.changes.timeOpen!, pending.changes.timeClose!);
      } else {
        await this.restaurantService.updateDays(pending.changes.days!, pending.changes.offDays!);
      }
      this.pendingOperation.set(null);
    } catch {
      // The service exposes the API error in its error signal for the page to display.
    }
  }
  closeDialogs(): void { this.activeDialog.set(null); this.pendingOperation.set(null); }
  statusLabel(status: string): string { return status === 'open' ? 'Open now' : status === 'repair' ? 'Under repair' : 'Closed'; }
  formatDay(day: string): string { return day.slice(0,3); }
}
