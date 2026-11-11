export interface ChefProfile {
  id: number;
  name: string;
  speciality: string;
  experience: number;
  requiredSalary: number;
}

export interface SearchRestaurant {
  id: number;
  name: string;
  address: string;
  capacity: number;
  status: string;
  nationality: string;
  budget: number | string;
  timeOpen?: string | null;
  timeClose?: string | null;
  operatingDays?: string[];
}

export interface ChefApplication {
  id: number;
  restaurantId: number;
  restaurantName: string;
  address: string;
  speciality?: string;
  salary: number;
  date: string;
  status: 'pending' | 'accepted' | 'refused';
}
