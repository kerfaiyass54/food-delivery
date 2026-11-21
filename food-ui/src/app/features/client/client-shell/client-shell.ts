import { Component, computed, inject } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { ClientsService } from '../../../core/services/commerce/clients';
import { SidebarNavComponent } from '../../../shared/components/sidebar-nav/sidebar-nav';

@Component({
  selector: 'app-client-shell',
  standalone: true,
  imports: [RouterOutlet, SidebarNavComponent],
  templateUrl: './client-shell.html',
  styleUrl: './client-shell.css',
})
export class ClientShellComponent {
  readonly data = inject(ClientsService);
  readonly name = computed(() => this.data.client()?.name ?? 'Your table');
  readonly navigation = [
    { label: 'Discover food', icon: 'cart', route: '/client/shop' },
    { label: 'Orders & purchases', icon: 'orders', route: '/client/orders' },
    { label: 'Your profile', icon: 'account', route: '/client/profile' },
  ];

  constructor() {
    void this.data.initializeClient();
  }
}
