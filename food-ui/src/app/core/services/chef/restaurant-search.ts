import { HttpClient, HttpErrorResponse, HttpParams } from '@angular/common/http';
import { Injectable, inject, signal } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { ApiConfigService } from '../api-config.service';
import { SearchRestaurant } from '../../../shared/models/chef.models';

interface RestaurantSearchDto {
  id: number;
  name: string;
  address: string;
  capacity: number;
  status: string;
  nationality: string | null;
  budget: number;
  timeOpen: string | null;
  timeClose: string | null;
  operatingDays: string[];
}

interface RestaurantSearchPage {
  items: RestaurantSearchDto[];
  totalPages: number;
}

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
      const filter = query.trim();
      const restaurants: RestaurantSearchDto[] = [];
      let page = 1;
      let totalPages = 1;

      while (page <= totalPages) {
        let params = new HttpParams().set('page', page).set('pageSize', 100);
        if (filter) params = params.set('filter', filter);

        const result = await firstValueFrom(
          this.http.get<RestaurantSearchPage>(`${config.apiBaseUrl}/restaurants`, { params })
        );
        restaurants.push(...result.items);
        totalPages = result.totalPages;
        page += 1;
      }

      this.restaurants.set(
        restaurants.map(item => ({
          id: item.id,
          name: item.name,
          address: item.address,
          capacity: item.capacity,
          status: item.status,
          nationality: item.nationality ?? '',
          budget: item.budget,
          timeOpen: item.timeOpen,
          timeClose: item.timeClose,
          operatingDays: item.operatingDays,
        }))
      );
    } catch (error) {
      this.searchError.set(this.message(error, 'Unable to search restaurants.'));
      this.restaurants.set([]);
    } finally { this.searchLoading.set(false); }
  }

  private message(error: unknown, fallback: string): string {
    return error instanceof HttpErrorResponse && error.error?.detail ? error.error.detail : fallback;
  }
}
