import { Routes } from '@angular/router';
import { OwnerShellComponent } from './owner-shell/owner-shell';

export const OWNER_ROUTES: Routes = [
  {
    path: '',
    component: OwnerShellComponent,
    children: [
      {
        path: '',
        pathMatch: 'full',
        redirectTo: 'restaurant',
      },
      {
        path: 'restaurant',
        loadComponent: () => import('./restaurant-details/restaurant-details')
          .then((module) => module.RestaurantDetailsComponent),
      },
      {
        path: 'chefs',
        loadComponent: () => import('./chefs/chefs')
          .then((module) => module.ChefsComponent),
      },
      {
        path: 'candidatures',
        loadComponent: () => import('./candidatures/candidatures')
          .then((module) => module.CandidaturesComponent),
      },
    ],
  },
];
