import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Injectable, inject, signal } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { ApiConfigService } from './api-config.service';
import { OwnerRestaurantService } from './owner/owner-restaurant.service';
import { ChefDetails, ChefSummary } from '../../shared/models/owner.models';

@Injectable({ providedIn: 'root' })
export class ChefService {
  private readonly http = inject(HttpClient);
  private readonly config = inject(ApiConfigService);
  private readonly ownerRestaurant = inject(OwnerRestaurantService);
  readonly chefs = signal<ChefSummary[]>([]);
  readonly loading = signal(false);
  readonly error = signal<string | null>(null);

  async load(): Promise<void> {
    const restaurantId = await this.waitForRestaurant();
    if (!restaurantId) return;
    this.loading.set(true); this.error.set(null);
    try {
      const base = (await this.config.get()).apiBaseUrl;
      this.chefs.set(await firstValueFrom(this.http.get<ChefSummary[]>(`${base}/restaurants/${restaurantId}/chefs`)));
    } catch (error) { this.error.set(this.message(error, 'Unable to load chefs.')); }
    finally { this.loading.set(false); }
  }

  async getDetails(chefId: number): Promise<ChefDetails | null> {
    const restaurantId = this.ownerRestaurant.getRestaurantId();
    if (!restaurantId) return null;
    try {
      const base = (await this.config.get()).apiBaseUrl;
      return await firstValueFrom(this.http.get<ChefDetails>(`${base}/restaurants/${restaurantId}/chefs/${chefId}`));
    } catch (error) { this.error.set(this.message(error, 'Unable to load chef details.')); return null; }
  }

  removeLocally(chefId: number): void { this.chefs.update((items) => items.filter((chef) => chef.id !== chefId)); }

  private async waitForRestaurant(): Promise<number | null> {
    if (this.ownerRestaurant.getRestaurantId()) return this.ownerRestaurant.getRestaurantId();
    await this.ownerRestaurant.initialize();
    return this.ownerRestaurant.getRestaurantId();
  }

  private message(error: unknown, fallback: string): string { return error instanceof HttpErrorResponse && error.error?.detail ? error.error.detail : fallback; }
}
