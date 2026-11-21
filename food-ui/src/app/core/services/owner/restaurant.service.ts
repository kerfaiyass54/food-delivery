import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Injectable, inject, signal } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { Observable } from 'rxjs';
import { ApiConfigService } from '../api-config.service';
import { OwnerRestaurantService } from './owner-restaurant.service';
import { RestaurantDetailsDto } from '../../../shared/dto/restaurant-details.dto';
import { WEEK_DAYS } from '../../../shared/constants/week-days';
import { mapRestaurantDetails } from '../../../shared/mappers/restaurant.mapper';

@Injectable({ providedIn: 'root' })
export class RestaurantService {
  private readonly http = inject(HttpClient);
  private readonly config = inject(ApiConfigService);
  private readonly ownerRestaurant = inject(OwnerRestaurantService);
  readonly loading = signal(false);
  readonly error = signal<string | null>(null);

  async updateCapacity(capacity: number): Promise<void> {
    await this.update((base, id) =>
      this.http.put<RestaurantDetailsDto>(`${base}/restaurants/${id}/capacity`, { capacity })
    );
  }

  async updateDays(days: string[], offDays: string[]): Promise<void> {
    const ids = (values: string[]) =>
      values.map(
        value => WEEK_DAYS.indexOf(value as (typeof WEEK_DAYS)[number]) + 1
      );
    await this.update((base, id) =>
      this.http.put<RestaurantDetailsDto>(`${base}/restaurants/${id}/days`, {
        operatingDayIds: ids(days),
        offDayIds: ids(offDays),
      })
    );
  }

  async updateOpeningHours(timeOpen: string, timeClose: string): Promise<void> {
    await this.update((base, id) =>
      this.http.put<RestaurantDetailsDto>(`${base}/restaurants/${id}/opening-hours`, {
        opensAt: `${timeOpen}:00`,
        closesAt: `${timeClose}:00`,
      })
    );
  }

  private async update(
    request: (base: string, id: number) => Observable<RestaurantDetailsDto>
  ): Promise<void> {
    const id = this.ownerRestaurant.getRestaurantId();
    if (!id) {
      this.error.set('Restaurant details are not available yet.');
      return;
    }
    this.loading.set(true);
    this.error.set(null);
    try {
      const config = await this.config.get();
      const response = await firstValueFrom(request(config.apiBaseUrl, id));
      this.ownerRestaurant.restaurant.set(mapRestaurantDetails(response));
    } catch (error) {
      this.error.set(this.message(error, 'Unable to update restaurant details.'));
      throw error;
    } finally {
      this.loading.set(false);
    }
  }

  private message(error: unknown, fallback: string): string {
    return error instanceof HttpErrorResponse && error.error?.detail
      ? error.error.detail
      : fallback;
  }
}
