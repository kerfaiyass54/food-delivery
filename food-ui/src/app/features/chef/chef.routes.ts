import { Routes } from '@angular/router';
import { ChefShellComponent } from './chef-shell/chef-shell';

export const CHEF_ROUTES: Routes = [
  {
    path: '',
    component: ChefShellComponent,
    children: [
      { path: '', pathMatch: 'full', redirectTo: 'details' },
      {
        path: 'details',
        loadComponent: () =>
          import('./chef-details/chef-details').then(module => module.ChefDetailsComponent),
      },
      {
        path: 'restaurant-search',
        loadComponent: () =>
          import('./restaurant-search/restaurant-search').then(
            module => module.RestaurantSearchComponent
          ),
      },
      {
        path: 'candidature-following',
        loadComponent: () =>
          import('./candidature-following/candidature-following').then(
            module => module.CandidatureFollowingComponent
          ),
      },
      {
        path: 'products',
        loadComponent: () =>
          import('./products/products').then(module => module.ChefProductsComponent),
      },
      {
        path: 'orders',
        loadComponent: () => import('./orders/orders').then(module => module.ChefOrdersComponent),
      },
      { path: 'restaurants', pathMatch: 'full', redirectTo: 'restaurant-search' },
      { path: 'candidatures', pathMatch: 'full', redirectTo: 'candidature-following' },
    ],
  },
];
