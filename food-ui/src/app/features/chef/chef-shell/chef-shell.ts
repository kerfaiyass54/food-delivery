import { Component, computed, inject } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { SidebarNavComponent } from '../../../shared/components/sidebar-nav/sidebar-nav';
import { ChefPortalService } from '../../../core/services/chef-portal.service';

@Component({
  selector: 'app-chef-shell',
  standalone: true,
  imports: [RouterOutlet, SidebarNavComponent],
  templateUrl: './chef-shell.html',
  styleUrl: './chef-shell.css',
})
export class ChefShellComponent {
  readonly data = inject(ChefPortalService);
  readonly name = computed(() => this.data.profile()?.name ?? 'Chef profile');
  readonly navigation = [
    { label: 'My details', icon: 'person', route: '/chef/details' },
    { label: 'Find a restaurant', icon: 'search', route: '/chef/restaurant-search' },
    { label: 'My applications', icon: 'receipt', route: '/chef/candidature-following' },
  ];
  constructor() {
    void this.data.loadProfile();
  }
}
