import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Injectable, inject, signal } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { ApiConfigService } from './api-config.service';
import { OwnerRestaurantService } from './owner/owner-restaurant.service';
import { Candidature, ChefDetails } from '../../shared/models/owner.models';

interface CandidatureDetailsDto {
  id: number;
  chefId: number;
  chefName: string;
  chefSpeciality: string;
  chefExperience: number;
  salary: number;
  date: string;
  status: string;
}

@Injectable({ providedIn: 'root' })
export class CandidatureService {
  private readonly http = inject(HttpClient);
  private readonly config = inject(ApiConfigService);
  private readonly ownerRestaurant = inject(OwnerRestaurantService);
  readonly candidatures = signal<Candidature[]>([]);
  readonly loading = signal(false);
  readonly error = signal<string | null>(null);

  async load(): Promise<void> {
    const restaurantId = await this.waitForRestaurant();
    if (!restaurantId) return;
    this.loading.set(true);
    this.error.set(null);
    try {
      const base = (await this.config.get()).apiBaseUrl;
      const response = await firstValueFrom(
        this.http.get<CandidatureDetailsDto[]>(`${base}/restaurants/${restaurantId}/candidatures`)
      );
      this.candidatures.set(response.map(item => this.map(item)));
    } catch (error) {
      this.error.set(this.message(error, 'Unable to load candidatures.'));
    } finally {
      this.loading.set(false);
    }
  }

  async getChef(candidatureId: number): Promise<ChefDetails | null> {
    const restaurantId = this.ownerRestaurant.getRestaurantId();
    if (!restaurantId) return null;
    try {
      const base = (await this.config.get()).apiBaseUrl;
      return await firstValueFrom(
        this.http.get<ChefDetails>(
          `${base}/restaurants/${restaurantId}/candidatures/${candidatureId}/chef`
        )
      );
    } catch (error) {
      this.error.set(this.message(error, 'Unable to load connected chef.'));
      return null;
    }
  }

  async refuse(candidatureId: number): Promise<void> {
    await this.mutate(candidatureId, 'refuse');
  }

  async accept(candidature: Candidature): Promise<void> {
    const restaurantId = this.ownerRestaurant.getRestaurantId();
    if (!restaurantId) return;
    try {
      const base = (await this.config.get()).apiBaseUrl;
      await firstValueFrom(
        this.http.post(`${base}/restaurants/${restaurantId}/chefs/${candidature.chefId}`, {})
      );
      this.candidatures.update(items =>
        items.map(item => (item.id === candidature.id ? { ...item, status: 'accepted' } : item))
      );
    } catch (error) {
      this.error.set(this.message(error, 'Unable to accept candidature.'));
      throw error;
    }
  }

  private async mutate(candidatureId: number, action: 'refuse'): Promise<void> {
    const restaurantId = this.ownerRestaurant.getRestaurantId();
    if (!restaurantId) return;
    try {
      const base = (await this.config.get()).apiBaseUrl;
      const updated = await firstValueFrom(
        this.http.put<CandidatureDetailsDto>(
          `${base}/restaurants/${restaurantId}/candidatures/${candidatureId}/${action}`,
          {}
        )
      );
      this.candidatures.update(items =>
        items.map(item => (item.id === candidatureId ? this.map(updated) : item))
      );
    } catch (error) {
      this.error.set(this.message(error, 'Unable to update candidature.'));
      throw error;
    }
  }

  private async waitForRestaurant(): Promise<number | null> {
    if (this.ownerRestaurant.getRestaurantId()) return this.ownerRestaurant.getRestaurantId();
    await this.ownerRestaurant.initialize();
    return this.ownerRestaurant.getRestaurantId();
  }

  private message(error: unknown, fallback: string): string {
    return error instanceof HttpErrorResponse && error.error?.detail
      ? error.error.detail
      : fallback;
  }

  private map(item: CandidatureDetailsDto): Candidature {
    const status = item.status.toLowerCase();
    return {
      id: item.id,
      chefId: item.chefId,
      chefName: item.chefName,
      chefSpeciality: item.chefSpeciality,
      chefExperience: item.chefExperience,
      salary: item.salary,
      date: item.date,
      status: status === 'accepted' || status === 'refused' ? status : 'pending',
    };
  }
}
