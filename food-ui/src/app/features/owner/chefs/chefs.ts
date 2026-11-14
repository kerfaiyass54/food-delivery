import { Component, computed, inject, signal } from '@angular/core';
import { ConfirmOperationDialogComponent } from '../confirm-operation-dialog/confirm-operation-dialog';
import { ChefDetailsDialogComponent } from './chef-details-dialog';
import { ChefService } from '../../../core/services/chef.service';
import { ChefDetails, ChefSummary } from '../../../shared/models/owner.models';

@Component({
  selector: 'app-chefs',
  standalone: true,
  imports: [ConfirmOperationDialogComponent, ChefDetailsDialogComponent],
  templateUrl: './chefs.html',
  styleUrl: './chefs.css',
})
export class ChefsComponent {
  readonly data = inject(ChefService);
  readonly specialtyCount = computed(
    () => new Set(this.data.chefs().map(chef => chef.speciality)).size
  );
  readonly selectedChef = signal<ChefDetails | null>(null);
  readonly chefToRemove = signal<ChefSummary | null>(null);

  constructor() {
    void this.data.load();
  }
  async showDetails(chef: ChefSummary): Promise<void> {
    this.selectedChef.set(await this.data.getDetails(chef.id));
  }
  askRemove(chef: ChefSummary): void {
    this.chefToRemove.set(chef);
  }
  closeDialogs(): void {
    this.selectedChef.set(null);
    this.chefToRemove.set(null);
  }
  removeChef(): void {
    const chef = this.chefToRemove();
    if (chef) this.data.removeLocally(chef.id);
    this.chefToRemove.set(null);
  }
}
