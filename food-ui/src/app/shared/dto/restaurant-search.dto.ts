export interface RestaurantSearchDto {
  id: number;
  name: string;
  address: string;
  capacity: number;
  status: string;
  nationality: string | null;
  budget: number;
  timeOpen: string | null;
  timeClose: string | null;
  operatingDays: string[];
}
