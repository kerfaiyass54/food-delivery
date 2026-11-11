import { Component, inject } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { SidebarNavComponent } from '../../../shared/components/sidebar-nav/sidebar-nav';
import { OwnerRestaurantService } from '../../../core/services/owner/owner-restaurant.service';

@Component({
  selector: 'app-owner-shell',
  standalone: true,
  imports: [RouterOutlet, SidebarNavComponent],
  templateUrl: './owner-shell.html',
  styleUrl: './owner-shell.css',
})
export class OwnerShellComponent {
  readonly data = inject(OwnerRestaurantService);
  readonly navigation = [
    { label: 'Restaurant', icon: 'shop', route: '/owner/restaurant' },
    { label: 'Chefs', icon: 'users', route: '/owner/chefs' },
    { label: 'Candidatures', icon: 'receipt', route: '/owner/candidatures' },
  ];

  constructor() {
    void this.data.initialize();
  }
}
