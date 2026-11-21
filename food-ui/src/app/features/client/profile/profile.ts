import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { ClientsService } from '../../../core/services/commerce/clients';
import { ClientStatus } from '../../../shared/models/client-status.model';
import { CreateClientRequest } from '../../../shared/models/create-client-request.model';

@Component({
  selector: 'app-client-profile',
  standalone: true,
  imports: [FormsModule, RouterLink],
  templateUrl: './profile.html',
  styleUrl: './profile.css',
})
export class ClientProfileComponent {
  readonly data = inject(ClientsService);
  readonly ClientStatus = ClientStatus;
  readonly saving = signal(false);
  readonly error = signal('');
  readonly success = signal(false);
  draft: CreateClientRequest = {
    name: '',
    city: '',
    nationality: '',
    budget: 0,
    weight: 0,
    status: ClientStatus.Normal,
  };

  constructor() {
    void this.data.initializeClient();
  }

  async createProfile(): Promise<void> {
    this.saving.set(true);
    this.error.set('');
    this.success.set(false);
    try {
      await this.data.create(this.draft);
      this.success.set(true);
    } catch (error) {
      this.error.set(this.message(error));
    } finally {
      this.saving.set(false);
    }
  }

  private message(error: unknown): string {
    const response = error as { error?: { detail?: string } };
    return response?.error?.detail ?? 'We could not create your profile. Please try again.';
  }
}
