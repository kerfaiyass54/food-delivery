import { ChefSummary } from './chef.model';

export interface ChefDetails extends ChefSummary {
  salary: number;
  restaurantId: number | null;
}
