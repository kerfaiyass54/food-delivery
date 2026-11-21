import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Injectable, inject, signal } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { ApiConfigService } from '../api-config.service';
import { RestaurantDetails } from '../../../shared/models/restaurant.model';
import { RestaurantDetailsDto } from '../../../shared/dto/restaurant-details.dto';
import { emptyRestaurantDetails, mapRestaurantDetails } from '../../../shared/mappers/restaurant.mapper';

@Injectable({ providedIn: 'root' })
export class OwnerRestaurantService {
  private readonly http = inject(HttpClient);
  private readonly config = inject(ApiConfigService);
  private readonly restaurantApiId = signal<number | null>(null);

  readonly ownerId = signal<number | null>(null);
  readonly restaurant = signal<RestaurantDetails>(emptyRestaurantDetails());
  readonly loading = signal(false);
  readonly error = signal<string | null>(null);

  async initialize(): Promise<void> {
    this.loading.set(true);
    this.error.set(null);
    try {
      const appConfig = await this.config.get();
      const ownerId = this.config.requireId(appConfig.ownerId, 'ownerId');
      const response = await firstValueFrom(
        this.http.get<RestaurantDetailsDto>(
          `${appConfig.apiBaseUrl}/owners/${ownerId}/restaurant`
        )
      );
      this.ownerId.set(ownerId);
      this.restaurantApiId.set(response.id);
      this.restaurant.set(mapRestaurantDetails(response));
    } catch (error) {
      this.setError(error, 'Unable to load restaurant details.');
    } finally {
      this.loading.set(false);
    }
  }

  loadRestaurant(): Promise<void> {
    return this.initialize();
  }

  getRestaurantId(): number | null {
    return this.restaurantApiId();
  }

  private setError(error: unknown, fallback: string): void {
    this.error.set(
      error instanceof HttpErrorResponse && error.error?.detail
        ? error.error.detail
        : error instanceof Error ? error.message : fallback
    );
  }
}
