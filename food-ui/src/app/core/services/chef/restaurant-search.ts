import { HttpClient, HttpErrorResponse, HttpParams } from '@angular/common/http';
import { Injectable, inject, signal } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { ApiConfigService } from '../api-config.service';
import { SearchRestaurant } from '../../../shared/models/chef.models';

@Injectable({ providedIn: 'root' })
export class RestaurantSearchService {
  private readonly http = inject(HttpClient);
  private readonly config = inject(ApiConfigService);
  readonly restaurants = signal<SearchRestaurant[]>([]);
  readonly searchLoading = signal(false);
  readonly searchError = signal<string | null>(null);

  async searchRestaurants(query: string): Promise<void> {
    this.searchLoading.set(true);
    this.searchError.set(null);
    try {
      const config = await this.config.get();
      const params = new HttpParams().set('query', query.trim());
      const results = await firstValueFrom(this.http.get<SearchRestaurant[]>(`${config.apiBaseUrl}/restaurants/search`, { params }));
      this.restaurants.set(results.map(item => ({ ...item, id: Number(item.id), capacity: Number(item.capacity ?? 0), budget: item.budget ?? '' })));
    } catch (error) {
      this.searchError.set(this.message(error, 'Unable to search restaurants.'));
      this.restaurants.set([]);
    } finally { this.searchLoading.set(false); }
  }

  private message(error: unknown, fallback: string): string {
    return error instanceof HttpErrorResponse && error.error?.detail ? error.error.detail : fallback;
  }
}
