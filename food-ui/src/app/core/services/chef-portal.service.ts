import { HttpClient, HttpErrorResponse, HttpParams } from '@angular/common/http';
import { Injectable, inject, signal } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { ApiConfigService } from './api-config.service';
import { ChefApplication, ChefProfile, SearchRestaurant } from '../../shared/models/chef.models';

@Injectable({ providedIn: 'root' })
export class ChefPortalService {
  private readonly http = inject(HttpClient);
  private readonly config = inject(ApiConfigService);
  private chefId = 0;

  readonly profile = signal<ChefProfile | null>(null);
  readonly profileLoading = signal(false);
  readonly profileError = signal<string | null>(null);
  readonly restaurants = signal<SearchRestaurant[]>([]);
  readonly searchLoading = signal(false);
  readonly searchError = signal<string | null>(null);
  readonly applications = signal<ChefApplication[]>([]);
  readonly applicationsLoading = signal(false);
  readonly applicationsError = signal<string | null>(null);
  readonly applying = signal(false);

  async loadProfile(): Promise<void> {
    if (this.profile() || this.profileLoading()) return;
    this.profileLoading.set(true); this.profileError.set(null);
    try {
      const config = await this.config.get(); this.chefId = config.chefId ?? 1;
      this.profile.set(this.mapProfile(await firstValueFrom(this.http.get<ChefProfile>(`${config.apiBaseUrl}/chefs/${this.chefId}`))));
    } catch (error) { this.profileError.set(this.message(error, 'Unable to load your chef profile.')); }
    finally { this.profileLoading.set(false); }
  }

  async updateProfile(changes: Pick<ChefProfile, 'speciality' | 'experience' | 'requiredSalary'>): Promise<void> {
    const profile = this.profile(); if (!profile) return;
    this.profileLoading.set(true); this.profileError.set(null);
    try {
      const config = await this.config.get();
      const updated = await firstValueFrom(this.http.put<ChefProfile>(`${config.apiBaseUrl}/chefs/${profile.id}`, { speciality: changes.speciality, experience: changes.experience, salary: changes.requiredSalary }));
      this.profile.set(this.mapProfile(updated));
    } catch (error) { this.profileError.set(this.message(error, 'Unable to save your profile changes.')); throw error; }
    finally { this.profileLoading.set(false); }
  }

  async searchRestaurants(query: string): Promise<void> {
    this.searchLoading.set(true); this.searchError.set(null);
    try {
      const config = await this.config.get();
      const params = new HttpParams().set('query', query.trim());
      const results = await firstValueFrom(this.http.get<SearchRestaurant[]>(`${config.apiBaseUrl}/restaurants/search`, { params }));
      this.restaurants.set(results.map(item => this.mapRestaurant(item)));
    } catch (error) { this.searchError.set(this.message(error, 'Unable to search restaurants.')); this.restaurants.set([]); }
    finally { this.searchLoading.set(false); }
  }

  async loadApplications(): Promise<void> {
    this.applicationsLoading.set(true); this.applicationsError.set(null);
    try {
      const config = await this.config.get(); this.chefId ||= config.chefId ?? 1;
      const items = await firstValueFrom(this.http.get<ChefApplication[]>(`${config.apiBaseUrl}/chefs/${this.chefId}/candidatures`));
      this.applications.set(items.map(item => this.mapApplication(item)));
    } catch (error) { this.applicationsError.set(this.message(error, 'Unable to load your applications.')); }
    finally { this.applicationsLoading.set(false); }
  }

  async applyToRestaurant(restaurantId: number): Promise<void> {
    const config = await this.config.get(); this.chefId ||= config.chefId ?? 1;
    const salary = this.profile()?.requiredSalary ?? 0;
    this.applying.set(true); this.searchError.set(null);
    try {
      await firstValueFrom(this.http.post(`${config.apiBaseUrl}/restaurants/${restaurantId}/candidatures`, { chefId: this.chefId, salary }));
      await this.loadApplications();
    } catch (error) { this.searchError.set(this.message(error, 'Unable to send your application.')); throw error; }
    finally { this.applying.set(false); }
  }

  private mapProfile(value: ChefProfile): ChefProfile {
    const dto = value as ChefProfile & { salary?: number };
    return { ...value, requiredSalary: Number(value.requiredSalary ?? dto.salary ?? 0) };
  }
  private mapRestaurant(value: SearchRestaurant): SearchRestaurant { return { ...value, id: Number(value.id), capacity: Number(value.capacity ?? 0), budget: value.budget ?? '' }; }
  private mapApplication(value: ChefApplication): ChefApplication {
    const status = String(value.status).toLowerCase();
    return { ...value, status: status === 'accepted' || status === 'refused' ? status : 'pending' };
  }
  private message(error: unknown, fallback: string): string { return error instanceof HttpErrorResponse && error.error?.detail ? error.error.detail : fallback; }
}
