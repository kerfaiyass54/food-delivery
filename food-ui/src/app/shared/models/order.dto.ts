import { OrderLineDto } from './order-line.dto';

export interface OrderDto {
  id: number;
  clientId: number;
  chefId: number;
  date: string;
  time: string;
  status: string;
  products: OrderLineDto[];
}
