import { ClientStatus } from './client-status.model';

export interface CreateClientRequest {
  name: string;
  city: string;
  nationality: string;
  budget: number;
  weight: number;
  status?: ClientStatus;
}
