import { Component } from '@angular/core';

@Component({
  selector: 'app-chefs',
  standalone: true,
  template: `
    <section class="owner-placeholder">
      <span class="eyebrow">OWNER WORKSPACE</span>
      <h1>Chefs</h1>
      <p>Manage the chefs connected to your restaurant.</p>
    </section>
  `,
  styles: [`.owner-placeholder{max-width:900px;margin:12vh auto;padding:48px;border:1px solid #e0e9df;border-radius:28px;background:#fff;box-shadow:0 20px 50px #3248350d}.eyebrow{color:#7c9b7e;font-size:10px;font-weight:800;letter-spacing:1.5px}.owner-placeholder h1{margin:14px 0 8px;color:#29392e;font-size:clamp(38px,5vw,64px);letter-spacing:-3px}.owner-placeholder p{color:#829082;font-size:14px}`],
})
export class ChefsComponent {}
