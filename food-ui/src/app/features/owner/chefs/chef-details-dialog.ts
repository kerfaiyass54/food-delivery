import { Component, input, output } from '@angular/core';

import { ChefDetails } from '../../../shared/models/owner.models';

@Component({
  selector: 'app-chef-details-dialog',
  standalone: true,
  template: `
    <div class="dialog-backdrop" (click)="closed.emit()" (keydown.escape)="closed.emit()">
      <section class="details-dialog" role="dialog" aria-modal="true" aria-label="Chef details" (click)="$event.stopPropagation()">
        <button class="close-button" type="button" aria-label="Close details" (click)="closed.emit()"><i class="bi bi-x-lg"></i></button>
        <div class="dialog-avatar">{{ chef().name.slice(0, 2).toUpperCase() }}</div><span class="dialog-kicker">CHEF PROFILE</span><h2>{{ chef().name }}</h2><p class="dialog-specialty">ID #{{ chef().id }} <span>·</span> {{ chef().speciality }} <span>·</span> {{ chef().experience }} years</p>
        <div class="dialog-info"><div><small>SALARY</small><b>{{ chef().salary }}</b></div><div><small>RESTAURANT</small><b>{{ chef().restaurantId ?? 'Unassigned' }}</b></div></div>
        <button class="dialog-done" type="button" (click)="closed.emit()">Done</button>
      </section>
    </div>
  `,
  styles: [`.dialog-backdrop{position:fixed;inset:0;z-index:1100;display:grid;place-items:center;padding:20px;background:#1c271f70;backdrop-filter:blur(8px);animation:veil .2s}.details-dialog{position:relative;width:min(100%,430px);padding:32px;border-radius:26px;background:#fff;box-shadow:0 25px 70px #15221940;text-align:center;animation:dialog-in .3s cubic-bezier(.2,.8,.2,1)}.close-button{position:absolute;top:17px;right:17px;width:32px;height:32px;border:0;border-radius:9px;background:#f3f6f2;color:#8c998e;cursor:pointer}.dialog-avatar{display:grid;place-items:center;width:68px;height:68px;margin:0 auto 17px;border-radius:20px;background:#e5f1e2;color:#57805e;font-size:19px;font-weight:800}.dialog-kicker{color:#87a089;font-size:9px;font-weight:800;letter-spacing:1.4px}.details-dialog h2{margin:9px 0 5px;color:#304034;font-size:25px}.dialog-specialty{margin:0;color:#8b988d;font-size:11px}.dialog-specialty span{margin:0 4px;color:#bec8bf}.dialog-info{display:grid;gap:10px;margin:25px 0;text-align:left}.dialog-info div{display:flex;justify-content:space-between;gap:20px;padding:11px 13px;border-radius:11px;background:#f6f9f5}.dialog-info small{color:#9aa69c;font-size:8px;font-weight:800;letter-spacing:1px}.dialog-info b{color:#526356;font-size:10px;font-weight:650}.dialog-done{width:100%;padding:11px;border:0;border-radius:11px;background:#4f8058;color:#fff;font-size:11px;font-weight:750;cursor:pointer}@keyframes veil{from{opacity:0}to{opacity:1}}@keyframes dialog-in{from{opacity:0;transform:translateY(15px) scale(.97)}to{opacity:1;transform:none}}`],
})
export class ChefDetailsDialogComponent {
  readonly chef = input.required<ChefDetails>();
  readonly closed = output<void>();
}
