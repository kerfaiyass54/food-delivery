import { Component, input } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { NavigationItem } from '../../models/navigation-item.model';
import { IconComponent } from '../icon/icon';

@Component({
  selector: 'app-sidebar-nav',
  standalone: true,
  imports: [IconComponent, RouterLink, RouterLinkActive],
  templateUrl: './sidebar-nav.html',
  styleUrl: './sidebar-nav.css',
})
export class SidebarNavComponent {
  items = input.required<NavigationItem[]>();
  profileName = input('Sophie Laurent');
  profileRole = input('Restaurant owner');
  brandSubtitle = input('KITCHEN STUDIO');
  brandLink = input('/owner/restaurant');

  iconName(icon: string): string {
    const icons: Record<string, string> = {
      grid: 'grid',
      shop: 'shop-window',
      receipt: 'receipt',
      menu: 'journal-text',
      users: 'people',
      chart: 'graph-up-arrow',
      settings: 'gear',
      person: 'person-vcard',
      search: 'search',
      cart: 'cart3',
      inventory: 'boxes',
      orders: 'receipt-cutoff',
      account: 'person-badge',
    };
    return icons[icon] ?? 'circle';
  }
}
