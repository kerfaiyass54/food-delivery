import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Injectable, inject, signal } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { Observable } from 'rxjs';
import { ApiConfigService } from './api-config.service';
import { OwnerRestaurantService } from './owner/owner-restaurant.service';
import { RestaurantDetails } from '../../shared/models/owner.models';

@Injectable({ providedIn: 'root' })
export class RestaurantService {
  private readonly http = inject(HttpClient);
  private readonly config = inject(ApiConfigService);
  private readonly ownerRestaurant = inject(OwnerRestaurantService);
  readonly loading = signal(false);
  readonly error = signal<string | null>(null);

  async updateCapacity(capacity: number): Promise<void> {
    await this.update((base, id) => this.http.put<RestaurantDetailsDto>(`${base}/restaurants/${id}/capacity`, { capacity }));
  }

  async updateDays(days: string[], offDays: string[]): Promise<void> {
    const ids = (values: string[]) => values.map((value) => ['Monday','Tuesday','Wednesday','Thursday','Friday','Saturday','Sunday'].indexOf(value) + 1);
    await this.update((base, id) => this.http.put<RestaurantDetailsDto>(`${base}/restaurants/${id}/days`, { operatingDayIds: ids(days), offDayIds: ids(offDays) }));
  }

  async updateOpeningHours(timeOpen: string, timeClose: string): Promise<void> {
    await this.update((base, id) => this.http.put<RestaurantDetailsDto>(`${base}/restaurants/${id}/opening-hours`, { opensAt: `${timeOpen}:00`, closesAt: `${timeClose}:00` }));
  }

  private async update(request: (base: string, id: number) => Observable<RestaurantDetailsDto>): Promise<void> {
    const id = this.ownerRestaurant.getRestaurantId();
    if (!id) { this.error.set('Restaurant details are not available yet.'); return; }
    this.loading.set(true); this.error.set(null);
    try {
      const config = await this.config.get();
      const response = await firstValueFrom(request(config.apiBaseUrl, id));
      this.ownerRestaurant.restaurant.set(this.mapRestaurant(response));
    } catch (error) {
      this.error.set(this.message(error, 'Unable to update restaurant details.'));
      throw error;
    } finally { this.loading.set(false); }
  }

  private mapRestaurant(dto: RestaurantDetailsDto): RestaurantDetails {
    return { apiId: dto.id, id: `RST-${dto.id}`, name: dto.name, address: dto.address, capacity: dto.capacity, dateOfAdd: dto.dateOfAdd, status: dto.status.toLowerCase() === 'open' || dto.status.toLowerCase() === 'repair' ? dto.status.toLowerCase() as 'open' | 'repair' : 'closed', timeOpen: dto.timeOpen?.slice(0, 5) ?? '--:--', timeClose: dto.timeClose?.slice(0, 5) ?? '--:--', nationality: dto.nationality ?? '', budget: dto.budget > 0 ? `$${dto.budget.toFixed(0)}` : '', days: dto.operatingDays.map((day) => day.name), offDays: dto.offDays.map((day) => day.name) };
  }

  private message(error: unknown, fallback: string): string { return error instanceof HttpErrorResponse && error.error?.detail ? error.error.detail : fallback; }
}

interface RestaurantDetailsDto { id: number; name: string; address: string; capacity: number; dateOfAdd: string; status: string; timeOpen: string | null; timeClose: string | null; nationality: string | null; budget: number; operatingDays: { id: number; name: string }[]; offDays: { id: number; name: string }[]; }
