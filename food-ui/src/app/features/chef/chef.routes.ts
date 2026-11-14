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
      { path: 'restaurants', pathMatch: 'full', redirectTo: 'restaurant-search' },
      { path: 'candidatures', pathMatch: 'full', redirectTo: 'candidature-following' },
    ],
  },
];
