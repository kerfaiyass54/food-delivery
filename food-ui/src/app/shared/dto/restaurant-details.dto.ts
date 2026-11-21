import { ApiDayDto } from './api-day.dto';

export interface RestaurantDetailsDto {
  id: number;
  name: string;
  address: string;
  capacity: number;
  dateOfAdd: string;
  status: string;
  timeOpen: string | null;
  timeClose: string | null;
  nationality: string | null;
  budget: number;
  operatingDays: ApiDayDto[];
  offDays: ApiDayDto[];
}
