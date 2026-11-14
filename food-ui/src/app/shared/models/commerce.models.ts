export enum ClientStatus {
  Sick = 0,
  Normal = 1,
  Diet = 2,
}

export interface CreateClientRequest {
  name: string;
  city: string;
  nationality: string;
  budget: number;
  weight: number;
  status?: ClientStatus;
}

export interface ClientDto {
  id: number;
  name: string;
  city: string;
  nationality: string;
  budget: number;
  weight: number;
  status: string;
}

export interface ProductDto {
  id: number;
  name: string;
  type: string;
  stock: number;
  cookDate: string;
  chefId: number;
}

export interface CookProductRequest {
  name: string;
  type: string;
  quantity: number;
}

export interface PurchaseRequest {
  productId: number;
  quantity: number;
  price: number;
}

export interface PurchaseDto {
  id: number;
  clientId: number;
  productId: number;
  date: string;
  price: number;
  quantity: number;
}

export interface OrderLineRequest {
  productId: number;
  quantity: number;
}

export interface CreateOrderRequest {
  products: OrderLineRequest[];
  /** 24-hour local time in HH:mm:ss format, as expected by .NET TimeOnly. */
  time: string;
}

export interface OrderLineDto {
  productId: number;
  productName: string;
  quantity: number;
}

export interface OrderDto {
  id: number;
  clientId: number;
  chefId: number;
  date: string;
  time: string;
  status: string;
  products: OrderLineDto[];
}
