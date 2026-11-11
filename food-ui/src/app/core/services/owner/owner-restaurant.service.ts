import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Injectable, inject, signal } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { ApiConfigService } from '../api-config.service';
import { RestaurantDetails, RestaurantStatus } from '../../../shared/models/owner.models';

interface ApiDay { id: number; name: string; }
interface RestaurantDetailsDto {
  id: number; name: string; address: string; capacity: number; dateOfAdd: string;
  status: string; timeOpen: string | null; timeClose: string | null;
  nationality: string | null; budget: number; operatingDays: ApiDay[]; offDays: ApiDay[];
}

@Injectable({ providedIn: 'root' })
export class OwnerRestaurantService {
  private readonly http = inject(HttpClient);
  private readonly config = inject(ApiConfigService);
  private readonly restaurantApiId = signal<number | null>(null);

  readonly ownerId = signal<number | null>(null);
  readonly restaurant = signal<RestaurantDetails>(this.emptyRestaurant());
  readonly loading = signal(false);
  readonly error = signal<string | null>(null);

  async initialize(): Promise<void> {
    this.loading.set(true);
    this.error.set(null);
    try {
      const appConfig = await this.config.get();
      const response = await firstValueFrom(this.http.get<RestaurantDetailsDto>(
        `${appConfig.apiBaseUrl}/owners/${appConfig.ownerId}/restaurant`,
      ));
      this.ownerId.set(appConfig.ownerId);
      this.restaurantApiId.set(response.id);
      this.restaurant.set(this.mapRestaurant(response));
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

  private mapRestaurant(dto: RestaurantDetailsDto): RestaurantDetails {
    return {
      apiId: dto.id, id: `RST-${dto.id}`, name: dto.name, address: dto.address,
      capacity: dto.capacity, dateOfAdd: dto.dateOfAdd, status: this.toStatus(dto.status),
      timeOpen: this.toTime(dto.timeOpen), timeClose: this.toTime(dto.timeClose),
      nationality: dto.nationality ?? '', budget: dto.budget > 0 ? `$${dto.budget.toFixed(0)}` : '',
      days: dto.operatingDays.map((day) => day.name), offDays: dto.offDays.map((day) => day.name),
    };
  }

  private emptyRestaurant(): RestaurantDetails {
    return { apiId: 0, id: '', name: '', address: '', capacity: 0, dateOfAdd: '', status: 'closed', timeOpen: '--:--', timeClose: '--:--', days: [], offDays: [], nationality: '', budget: '' };
  }

  private toStatus(status: string): RestaurantStatus {
    const value = status.toLowerCase();
    return value === 'open' || value === 'repair' ? value : 'closed';
  }

  private toTime(value: string | null): string { return value ? value.slice(0, 5) : '--:--'; }

  private setError(error: unknown, fallback: string): void {
    this.error.set(error instanceof HttpErrorResponse && error.error?.detail ? error.error.detail : fallback);
  }
}
