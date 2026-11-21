import { RestaurantDetailsDto } from '../dto/restaurant-details.dto';
import { RestaurantDetails } from '../models/restaurant.model';
import { RestaurantStatus } from '../models/restaurant-status.model';

export function mapRestaurantDetails(dto: RestaurantDetailsDto): RestaurantDetails {
  return {
    apiId: dto.id,
    id: `RST-${dto.id}`,
    name: dto.name,
    address: dto.address,
    capacity: dto.capacity,
    dateOfAdd: dto.dateOfAdd,
    status: toRestaurantStatus(dto.status),
    timeOpen: dto.timeOpen?.slice(0, 5) ?? '--:--',
    timeClose: dto.timeClose?.slice(0, 5) ?? '--:--',
    nationality: dto.nationality ?? '',
    budget: dto.budget > 0 ? `$${dto.budget.toFixed(0)}` : '',
    days: dto.operatingDays.map(day => day.name),
    offDays: dto.offDays.map(day => day.name),
  };
}

export function emptyRestaurantDetails(): RestaurantDetails {
  return {
    apiId: 0,
    id: '',
    name: '',
    address: '',
    capacity: 0,
    dateOfAdd: '',
    status: 'closed',
    timeOpen: '--:--',
    timeClose: '--:--',
    days: [],
    offDays: [],
    nationality: '',
    budget: '',
  };
}

function toRestaurantStatus(status: string): RestaurantStatus {
  const value = status.toLowerCase();
  return value === 'open' || value === 'repair' ? value : 'closed';
}
