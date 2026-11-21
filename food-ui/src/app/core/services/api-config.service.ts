import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { AppConfig } from '../../shared/models/app-config.model';

@Injectable({ providedIn: 'root' })
export class ApiConfigService {
  private readonly http = inject(HttpClient);
  private configRequest?: Promise<AppConfig>;

  get(): Promise<AppConfig> {
    if (!this.configRequest) {
      this.configRequest = firstValueFrom(this.http.get<AppConfig>('/config.json')).catch(error => {
        this.configRequest = undefined;
        throw error;
      });
    }

    return this.configRequest;
  }
}
