import { Component, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { ChefCandidaturesService } from '../../../core/services/chef/chef-candidatures';
import { ChefProfileService } from '../../../core/services/chef/chef-profile';
import { RestaurantSearchService } from '../../../core/services/chef/restaurant-search';
import { SearchRestaurant } from '../../../shared/models/chef.models';
import { ConfirmOperationDialogComponent } from '../../owner/confirm-operation-dialog/confirm-operation-dialog';

@Component({
  selector: 'app-restaurant-search',
  standalone: true,
  imports: [FormsModule, RouterLink, ConfirmOperationDialogComponent],
  templateUrl: './restaurant-search.html',
  styleUrls: ['./restaurant-search.css'],
})
export class RestaurantSearchComponent {
  readonly data = inject(RestaurantSearchService);
  readonly applications = inject(ChefCandidaturesService);
  readonly profile = inject(ChefProfileService);
  query = '';
  readonly searched = signal(false);
  readonly selectedRestaurant = signal<SearchRestaurant | null>(null);
  readonly confirmRestaurant = signal<SearchRestaurant | null>(null);
  readonly successMessage = signal('');
  readonly appliedRestaurantIds = computed(
    () => new Set(this.applications.applications().map(item => item.restaurantId))
  );
  constructor() {
    void this.profile.loadProfile();
    void this.applications.loadApplications();
  }
  async search(): Promise<void> {
    this.searched.set(true);
    this.successMessage.set('');
    await this.data.searchRestaurants(this.query);
  }
  showDetails(restaurant: SearchRestaurant): void {
    this.selectedRestaurant.set(restaurant);
  }
  askToApply(restaurant: SearchRestaurant): void {
    this.selectedRestaurant.set(null);
    this.confirmRestaurant.set(restaurant);
  }
  closeDialogs(): void {
    this.selectedRestaurant.set(null);
    this.confirmRestaurant.set(null);
  }
  async apply(): Promise<void> {
    const restaurant = this.confirmRestaurant();
    if (!restaurant) return;
    try {
      await this.applications.applyToRestaurant(restaurant.id);
      this.successMessage.set(`Your application to ${restaurant.name} has been sent.`);
      this.confirmRestaurant.set(null);
    } catch {
      /* the service exposes the request error on the page */
    }
  }
  budgetLabel(value: number | string): string {
    if (typeof value === 'string') return value || 'Budget not listed';
    return value > 0
      ? '$'.repeat(Math.max(1, Math.min(4, Math.round(value))))
      : 'Budget not listed';
  }
  time(value?: string | null): string {
    return value?.slice(0, 5) ?? '—';
  }
  isApplied(id: number): boolean {
    return this.appliedRestaurantIds().has(id);
  }
}
