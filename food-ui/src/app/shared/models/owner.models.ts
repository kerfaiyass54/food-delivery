export type RestaurantStatus = 'open' | 'closed' | 'repair';
export type CandidatureStatus = 'accepted' | 'refused' | 'pending';

export interface AppConfig {
  apiBaseUrl: string;
  ownerId: number;
}

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

export interface ChefSummary {
  id: number;
  name: string;
  speciality: string;
  experience: number;
}

export interface ChefDetails extends ChefSummary {
  salary: number;
  restaurantId: number | null;
}

export interface Candidature {
  id: number;
  chefId: number;
  chefName: string;
  chefSpeciality: string;
  chefExperience: number;
  salary: number;
  date: string;
  status: CandidatureStatus;
}
