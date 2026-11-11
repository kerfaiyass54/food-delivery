import { Component, computed, inject, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { ConfirmOperationDialogComponent } from '../confirm-operation-dialog/confirm-operation-dialog';
import { CandidatureDetailsDialogComponent } from './candidature-details-dialog';
import { CandidatureService } from '../../../core/services/candidature.service';
import { Candidature } from '../../../shared/models/owner.models';

@Component({
  selector: 'app-candidatures',
  standalone: true,
  imports: [DatePipe, ConfirmOperationDialogComponent, CandidatureDetailsDialogComponent],
  templateUrl: './candidatures.html',
  styleUrl: './candidatures.css',
})
export class CandidaturesComponent {
  readonly data = inject(CandidatureService);
  readonly pendingCount = computed(() => this.data.candidatures().filter((item) => item.status === 'pending').length);
  readonly acceptedCount = computed(() => this.data.candidatures().filter((item) => item.status === 'accepted').length);
  readonly pendingAction = signal<{ item: Candidature; action: 'approve' | 'refuse' } | null>(null);
  readonly selectedCandidature = signal<Candidature | null>(null);

  constructor() { void this.data.load(); }
  ask(item: Candidature, action: 'approve' | 'refuse'): void { this.pendingAction.set({ item, action }); }
  showDetails(item: Candidature): void { this.selectedCandidature.set(item); }
  closeDialog(): void { this.pendingAction.set(null); this.selectedCandidature.set(null); }
  async applyAction(): Promise<void> {
    const pending = this.pendingAction();
    if (!pending) return;
    try {
      if (pending.action === 'approve') await this.data.accept(pending.item);
      else await this.data.refuse(pending.item.id);
      this.pendingAction.set(null);
    } catch { /* the service exposes the message in the page notification */ }
  }
}
