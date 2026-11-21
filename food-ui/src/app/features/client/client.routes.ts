import { Routes } from '@angular/router';
import { ClientShellComponent } from './client-shell/client-shell';

export const CLIENT_ROUTES: Routes = [
  {
    path: '',
    component: ClientShellComponent,
    children: [
      { path: '', pathMatch: 'full', redirectTo: 'shop' },
      {
        path: 'shop',
        loadComponent: () => import('./shop/shop').then(module => module.ClientShopComponent),
      },
      {
        path: 'orders',
        loadComponent: () => import('./orders/orders').then(module => module.ClientOrdersComponent),
      },
      {
        path: 'profile',
        loadComponent: () =>
          import('./profile/profile').then(module => module.ClientProfileComponent),
      },
    ],
  },
];
