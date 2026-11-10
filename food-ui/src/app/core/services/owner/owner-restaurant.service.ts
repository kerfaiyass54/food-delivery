import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Injectable, inject, signal } from '@angular/core';
import { firstValueFrom, Observable } from 'rxjs';
import { RestaurantDetails, RestaurantStatus } from '../../../features/owner/owner.models';

interface AppConfig {
  apiBaseUrl: string;
  ownerId: number;
}

interface ApiDay {
  id: number;
  name: string;
}

interface RestaurantDetailsDto {
  id: number;
  name: string;
  address: string;
  capacity: number;
  dateOfAdd: string;
  status: string;
  timeOpen: string | null;
  timeClose: string | null;
  nationality: string | null;
  budget: number;
  operatingDays: ApiDay[];
  offDays: ApiDay[];
}

@Injectable({ providedIn: 'root' })
export class OwnerRestaurantService {
  private readonly http = inject(HttpClient);
  private readonly restaurantApiId = signal<number | null>(null);
  private config?: AppConfig;

  readonly ownerId = signal<number | null>(null);
  readonly restaurant = signal<RestaurantDetails>({
    id: '',
    name: '',
    address: '',
    capacity: 0,
    dateOfAdd: '',
    status: 'closed',
    timeOpen: '--:--',
    timeClose: '--:--',
    days: [],
    offDays: [],
    nationality: '',
    budget: '',
  });
  readonly loading = signal(false);
  readonly error = signal<string | null>(null);

  async initialize(): Promise<void> {
    await this.loadRestaurant();
  }

  async loadRestaurant(): Promise<void> {
    this.loading.set(true);
    this.error.set(null);
    try {
      const config = await this.getConfig();
      const response = await firstValueFrom(
        this.http.get<RestaurantDetailsDto>(`${config.apiBaseUrl}/owners/${config.ownerId}/restaurant`),
      );
      this.ownerId.set(config.ownerId);
      this.restaurantApiId.set(response.id);
      this.restaurant.set(this.toRestaurantDetails(response));
    } catch (error) {
      this.setError(error, 'Unable to load restaurant details.');
    } finally {
      this.loading.set(false);
    }
  }

  async updateCapacity(capacity: number): Promise<void> {
    await this.update((config, restaurantId) => this.http.put<RestaurantDetailsDto>(
      `${config.apiBaseUrl}/restaurants/${restaurantId}/capacity`,
      { capacity },
    ));
  }

  async updateDays(days: string[], offDays: string[]): Promise<void> {
    await this.update((config, restaurantId) => this.http.put<RestaurantDetailsDto>(
      `${config.apiBaseUrl}/restaurants/${restaurantId}/days`,
      { operatingDayIds: this.dayIds(days), offDayIds: this.dayIds(offDays) },
    ));
  }

  async updateOpeningHours(timeOpen: string, timeClose: string): Promise<void> {
    await this.update((config, restaurantId) => this.http.put<RestaurantDetailsDto>(
      `${config.apiBaseUrl}/restaurants/${restaurantId}/opening-hours`,
      { opensAt: `${timeOpen}:00`, closesAt: `${timeClose}:00` },
    ));
  }

  private async getConfig(): Promise<AppConfig> {
    if (!this.config) {
      this.config = await firstValueFrom(this.http.get<AppConfig>('/config.json'));
    }
    return this.config;
  }

  private async update(
    request: (config: AppConfig, restaurantId: number) => Observable<RestaurantDetailsDto>,
  ): Promise<void> {
    const config = await this.getConfig();
    const restaurantId = this.restaurantApiId();
    if (restaurantId === null) {
      throw new Error('Restaurant details have not been loaded.');
    }

    this.loading.set(true);
    this.error.set(null);
    try {
      const response = await firstValueFrom(request(config, restaurantId));
      this.restaurant.set(this.toRestaurantDetails(response));
    } catch (error) {
      this.setError(error, 'Unable to update restaurant details.');
      throw error;
    } finally {
      this.loading.set(false);
    }
  }

  private dayIds(days: string[]): number[] {
    const ids = new Map(['Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday', 'Sunday']
      .map((day, index) => [day, index + 1]));
    return days.map((day) => ids.get(day)).filter((id): id is number => id !== undefined);
  }

  private toRestaurantDetails(dto: RestaurantDetailsDto): RestaurantDetails {
    return {
      id: `RST-${dto.id}`,
      name: dto.name,
      address: dto.address,
      capacity: dto.capacity,
      dateOfAdd: dto.dateOfAdd,
      status: this.toStatus(dto.status),
      timeOpen: this.toTime(dto.timeOpen),
      timeClose: this.toTime(dto.timeClose),
      nationality: dto.nationality ?? '',
      budget: dto.budget > 0 ? `$${dto.budget.toFixed(0)}` : '',
      days: dto.operatingDays.map((day) => day.name),
      offDays: dto.offDays.map((day) => day.name),
    };
  }

  private toStatus(status: string): RestaurantStatus {
    const normalized = status.toLowerCase();
    return normalized === 'open' || normalized === 'repair' ? normalized : 'closed';
  }

  private toTime(value: string | null): string {
    return value ? value.slice(0, 5) : '--:--';
  }

  private setError(error: unknown, fallback: string): void {
    if (error instanceof HttpErrorResponse && error.error?.detail) {
      this.error.set(error.error.detail);
      return;
    }
    this.error.set(fallback);
  }
}
