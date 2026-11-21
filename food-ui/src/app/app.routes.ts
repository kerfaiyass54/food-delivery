import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: 'owner',
    loadChildren: () => import('./features/owner/owner.routes').then(module => module.OWNER_ROUTES),
  },
  {
    path: 'chef',
    loadChildren: () => import('./features/chef/chef.routes').then(module => module.CHEF_ROUTES),
  },
  {
    path: 'client',
    loadChildren: () => import('./features/client/client.routes').then(module => module.CLIENT_ROUTES),
  },
  { path: '', pathMatch: 'full', redirectTo: 'owner/restaurant' },
  { path: '**', redirectTo: 'owner/restaurant' },
];
