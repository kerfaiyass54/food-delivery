export interface SearchRestaurant {
  id: number;
  name: string;
  address: string;
  capacity: number;
  status: string;
  nationality: string;
  budget: number;
  timeOpen: string | null;
  timeClose: string | null;
  operatingDays: string[];
}
