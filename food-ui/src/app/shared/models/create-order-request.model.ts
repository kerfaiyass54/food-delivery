import { OrderLineRequest } from './order-line-request.model';

export interface CreateOrderRequest {
  products: OrderLineRequest[];
  time: string;
}
