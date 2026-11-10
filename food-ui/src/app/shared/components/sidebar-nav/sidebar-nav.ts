import { Component, input } from '@angular/core';
import { NavigationItem } from '../../models/dashboard.model';
import { IconComponent } from '../icon/icon';

@Component({ selector:'app-sidebar-nav', standalone:true, imports:[IconComponent], templateUrl: './sidebar-nav.html', styleUrl:'./sidebar-nav.css' })
export class SidebarNavComponent {
  items = input.required<NavigationItem[]>();
  active = input('Overview');

  iconName(icon: string): string {
    const icons: Record<string, string> = { grid: 'grid', receipt: 'receipt', menu: 'journal-text', users: 'people', chart: 'graph-up-arrow', settings: 'gear' };
    return icons[icon] ?? 'circle';
  }
}

