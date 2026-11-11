import { Component, input, output } from '@angular/core';
import { DatePipe } from '@angular/common';
import { Candidature } from '../../../shared/models/owner.models';

@Component({
  selector: 'app-candidature-details-dialog',
  standalone: true,
  imports: [DatePipe],
  template: `
    <div class="dialog-backdrop" (click)="closed.emit()" (keydown.escape)="closed.emit()">
      <section class="details-dialog" role="dialog" aria-modal="true" aria-label="Candidature and chef details" (click)="$event.stopPropagation()">
        <button class="close-button" type="button" aria-label="Close details" (click)="closed.emit()"><i class="bi bi-x-lg"></i></button>
        <div class="dialog-avatar"><i class="bi bi-person-check"></i></div>
        <span class="dialog-kicker">CANDIDATURE DETAILS</span>
        <h2>Candidature #{{ candidature().id }}</h2>
        <div class="detail-block">
          <div class="block-heading"><i class="bi bi-file-earmark-text"></i><span>Application</span></div>
          <div class="detail-grid"><div><small>STATUS</small><b [class]="'status-' + candidature().status">{{ candidature().status }}</b></div><div><small>SALARY</small><b>{{ candidature().salary }}</b></div><div><small>DATE</small><b>{{ candidature().date | date:'mediumDate' }}</b></div></div>
        </div>
        <div class="detail-block chef-block">
          <div class="block-heading"><i class="bi bi-person-badge"></i><span>Connected chef</span></div>
          <div class="chef-details"><div><small>ID</small><b>#{{ candidature().chefId }}</b></div><div><small>NAME</small><b>{{ candidature().chefName }}</b></div><div><small>SPECIALITY</small><b>{{ candidature().chefSpeciality }}</b></div><div><small>EXPERIENCE</small><b>{{ candidature().chefExperience }} years</b></div><div><small>SALARY</small><b>{{ candidature().salary }}</b></div></div>
        </div>
        <button class="dialog-done" type="button" (click)="closed.emit()">Close details</button>
      </section>
    </div>
  `,
  styles: [`.dialog-backdrop{position:fixed;inset:0;z-index:1100;display:grid;place-items:center;padding:20px;background:#1c271f70;backdrop-filter:blur(8px);animation:veil .2s}.details-dialog{position:relative;width:min(100%,460px);padding:32px;border-radius:26px;background:#fff;box-shadow:0 25px 70px #15221940;text-align:left;animation:dialog-in .3s cubic-bezier(.2,.8,.2,1)}.close-button{position:absolute;top:17px;right:17px;width:32px;height:32px;border:0;border-radius:9px;background:#f3f6f2;color:#8c998e;cursor:pointer}.dialog-avatar{display:grid;place-items:center;width:58px;height:58px;margin-bottom:16px;border-radius:18px;background:#e5f1e2;color:#57805e;font-size:23px}.dialog-kicker{color:#87a089;font-size:9px;font-weight:800;letter-spacing:1.4px}.details-dialog h2{margin:9px 0 22px;color:#304034;font-size:25px}.detail-block{padding:16px;border:1px solid #e5eee3;border-radius:15px;background:#f8fbf7}.detail-block+.detail-block{margin-top:10px}.block-heading{display:flex;align-items:center;gap:8px;margin-bottom:14px;color:#628368;font-size:10px;font-weight:800;letter-spacing:.8px;text-transform:uppercase}.block-heading i{font-size:14px}.detail-grid{display:grid;grid-template-columns:repeat(3,1fr);gap:10px}.detail-grid div{display:grid;gap:5px}.detail-grid small{color:#a0aba1;font-size:7px;font-weight:800;letter-spacing:.8px}.detail-grid b{color:#516154;font-size:10px}.detail-grid b.status-accepted{color:#4f8058}.detail-grid b.status-refused{color:#aa665d}.detail-grid b.status-pending{color:#a8794e}  .chef-block strong{color:#3f5244;font-size:15px}.chef-block p{margin:6px 0 0;color:#8b988d;font-size:10px}.chef-details{display:grid;grid-template-columns:repeat(2,1fr);gap:10px}.chef-details div{display:grid;gap:4px;padding:10px;border-radius:10px;background:#fff}.chef-details small{color:#a0aba1;font-size:7px;font-weight:800;letter-spacing:.8px}.chef-details b{color:#516154;font-size:10px}.dialog-done{width:100%;margin-top:17px;padding:11px;border:0;border-radius:11px;background:#4f8058;color:#fff;font-size:11px;font-weight:750;cursor:pointer}@keyframes veil{from{opacity:0}to{opacity:1}}@keyframes dialog-in{from{opacity:0;transform:translateY(15px) scale(.97)}to{opacity:1;transform:none}}`],
})
export class CandidatureDetailsDialogComponent {
  readonly candidature = input.required<Candidature>();
  readonly closed = output<void>();
}
