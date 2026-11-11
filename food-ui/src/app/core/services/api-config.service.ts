import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { AppConfig } from '../../shared/models/owner.models';

@Injectable({ providedIn: 'root' })
export class ApiConfigService {
  private readonly http = inject(HttpClient);
  private config?: AppConfig;

  async get(): Promise<AppConfig> {
    if (!this.config) {
      this.config = await firstValueFrom(this.http.get<AppConfig>('/config.json'));
    }
    return this.config;
  }
}
