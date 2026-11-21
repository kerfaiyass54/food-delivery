import { RestaurantSearchDto } from './restaurant-search.dto';

export interface RestaurantSearchPageDto {
  items: RestaurantSearchDto[];
  totalPages: number;
}
