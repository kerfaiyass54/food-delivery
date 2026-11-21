import { RestaurantDetails } from '../../../shared/models/restaurant.model';
import { DialogKind } from './dialog-kind.model';

export interface PendingOperation {
  kind: DialogKind;
  title: string;
  message: string;
  changes: Partial<RestaurantDetails>;
}
