import { RestaurantStatus } from './restaurant-status.model';

export interface RestaurantDetails {
  id: string;
  apiId: number;
  address: string;
  name: string;
  capacity: number;
  dateOfAdd: string;
  status: RestaurantStatus;
  timeOpen: string;
  timeClose: string;
  days: string[];
  offDays: string[];
  nationality: string;
  budget: string;
}
