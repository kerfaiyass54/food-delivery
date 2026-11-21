export interface ChefApplication {
  id: number;
  restaurantId: number;
  restaurantName: string;
  address: string;
  speciality?: string;
  chefSpeciality?: string;
  salary: number;
  date: string;
  status: 'pending' | 'accepted' | 'refused';
}
