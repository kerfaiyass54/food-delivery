import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Injectable, inject, signal } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { ApiConfigService } from '../api-config.service';
import { ChefProfile } from '../../../shared/models/chef.models';

@Injectable({ providedIn: 'root' })
export class ChefProfileService {
  private readonly http = inject(HttpClient);
  private readonly config = inject(ApiConfigService);
  private chefId = 0;
  readonly profile = signal<ChefProfile | null>(null);
  readonly profileLoading = signal(false);
  readonly profileError = signal<string | null>(null);

  async loadProfile(): Promise<void> {
    if (this.profile() || this.profileLoading()) return;
    this.profileLoading.set(true);
    this.profileError.set(null);
    try {
      const config = await this.config.get();
      this.chefId = config.chefId ?? 1;
      const profile = await firstValueFrom(this.http.get<ChefProfile>(`${config.apiBaseUrl}/chefs/${this.chefId}`));
      this.profile.set(this.mapProfile(profile));
    } catch (error) {
      this.profileError.set(this.message(error, 'Unable to load your chef profile.'));
    } finally { this.profileLoading.set(false); }
  }

  async updateProfile(changes: Pick<ChefProfile, 'speciality' | 'experience' | 'requiredSalary'>): Promise<void> {
    await this.ensureChefId();
    this.profileLoading.set(true);
    this.profileError.set(null);
    try {
      const config = await this.config.get();
      const updated = await firstValueFrom(this.http.put<ChefProfile>(
        `${config.apiBaseUrl}/chefs/${this.chefId}/profile`,
        { speciality: changes.speciality, experience: changes.experience, salary: changes.requiredSalary },
      ));
      this.profile.set(this.mapProfile(updated));
    } catch (error) {
      this.profileError.set(this.message(error, 'Unable to save your profile changes.'));
      throw error;
    } finally { this.profileLoading.set(false); }
  }

  private async ensureChefId(): Promise<void> {
    if (!this.chefId) { const config = await this.config.get(); this.chefId = config.chefId ?? 1; }
  }
  private mapProfile(value: ChefProfile): ChefProfile {
    const dto = value as ChefProfile & { salary?: number };
    return { ...value, requiredSalary: Number(value.requiredSalary ?? dto.salary ?? 0) };
  }
  private message(error: unknown, fallback: string): string {
    return error instanceof HttpErrorResponse && error.error?.detail ? error.error.detail : fallback;
  }
}
