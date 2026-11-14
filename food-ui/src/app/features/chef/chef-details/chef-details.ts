import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { ChefProfileService } from '../../../core/services/chef/chef-profile';

@Component({
  selector: 'app-chef-details',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './chef-details.html',
  styleUrls: ['./chef-details.css'],
})
export class ChefDetailsComponent {
  readonly data = inject(ChefProfileService);
  readonly editing = signal(false);
  readonly saved = signal(false);
  draft = { speciality: '', experience: 0, requiredSalary: 0 };
  constructor() {
    void this.load();
  }
  async load(): Promise<void> {
    await this.data.loadProfile();
    this.syncDraft();
  }
  startEditing(): void {
    this.syncDraft();
    this.saved.set(false);
    this.editing.set(true);
  }
  cancelEditing(): void {
    this.editing.set(false);
    this.syncDraft();
  }
  async save(): Promise<void> {
    try {
      await this.data.updateProfile(this.draft);
      this.editing.set(false);
      this.saved.set(true);
    } catch {
      /* the service exposes its message in the profile error signal */
    }
  }
  private syncDraft(): void {
    const profile = this.data.profile();
    if (profile)
      this.draft = {
        speciality: profile.speciality,
        experience: profile.experience,
        requiredSalary: profile.requiredSalary,
      };
  }
}
