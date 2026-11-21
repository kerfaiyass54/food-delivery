import { CandidatureStatus } from './candidature-status.model';

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
