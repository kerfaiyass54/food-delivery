import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Injectable, inject, signal } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { ApiConfigService } from '../api-config.service';
import { ChefApplication } from '../../../shared/models/chef.models';
import { ChefProfileService } from './chef-profile';

@Injectable({ providedIn: 'root' })
export class ChefCandidaturesService {
  private readonly http = inject(HttpClient);
  private readonly config = inject(ApiConfigService);
  private readonly profile = inject(ChefProfileService);
  private chefId = 0;
  readonly applications = signal<ChefApplication[]>([]);
  readonly applicationsLoading = signal(false);
  readonly applicationsError = signal<string | null>(null);
  readonly applying = signal(false);

  async loadApplications(): Promise<void> {
    this.applicationsLoading.set(true);
    this.applicationsError.set(null);
    try {
      const config = await this.config.get();
      this.chefId ||= config.chefId ?? 1;
      const items = await firstValueFrom(this.http.get<ChefApplication[]>(`${config.apiBaseUrl}/chefs/${this.chefId}/candidatures`));
      this.applications.set(items.map(item => this.mapApplication(item)));
    } catch (error) {
      this.applicationsError.set(this.message(error, 'Unable to load your applications.'));
    } finally { this.applicationsLoading.set(false); }
  }

  async applyToRestaurant(restaurantId: number): Promise<void> {
    const config = await this.config.get();
    this.chefId ||= config.chefId ?? 1;
    const salary = this.profile.profile()?.requiredSalary ?? 0;
    this.applying.set(true);
    this.applicationsError.set(null);
    try {
      await firstValueFrom(this.http.post(`${config.apiBaseUrl}/chefs/${this.chefId}/candidatures`, { restaurantId, salary }));
      await this.loadApplications();
    } catch (error) {
      this.applicationsError.set(this.message(error, 'Unable to send your application.'));
      throw error;
    } finally { this.applying.set(false); }
  }

  private mapApplication(value: ChefApplication): ChefApplication {
    const status = String(value.status).toLowerCase();
    return { ...value, status: status === 'accepted' || status === 'refused' ? status : 'pending' };
  }
  private message(error: unknown, fallback: string): string {
    return error instanceof HttpErrorResponse && error.error?.detail ? error.error.detail : fallback;
  }
}
