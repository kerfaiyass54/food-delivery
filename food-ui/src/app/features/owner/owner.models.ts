export type RestaurantStatus = 'open' | 'closed' | 'repair';

export interface RestaurantDetails {
  id: string;
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
