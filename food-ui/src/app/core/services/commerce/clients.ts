import { HttpClient } from '@angular/common/http';
import { Injectable, inject, signal } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { ApiConfigService } from '../api-config.service';
import { ClientDto } from '../../../shared/models/client.dto';
import { CreateClientRequest } from '../../../shared/models/create-client-request.model';
import { CreateOrderRequest } from '../../../shared/models/create-order-request.model';
import { OrderDto } from '../../../shared/models/order.dto';
import { ProductDto } from '../../../shared/models/product.dto';
import { PurchaseDto } from '../../../shared/models/purchase.dto';
import { PurchaseRequest } from '../../../shared/models/purchase-request.model';

@Injectable({ providedIn: 'root' })
export class ClientsService {
  private readonly http = inject(HttpClient);
  private readonly config = inject(ApiConfigService);
  private static readonly clientStorageKey = 'food-delivery.client-id';
  readonly clientId = signal<number | null>(null);
  readonly client = signal<ClientDto | null>(null);

  async initializeClient(): Promise<number | null> {
    let configuredId: number | undefined;
    try {
      configuredId = (await this.config.get()).clientId;
    } catch {
      configuredId = undefined;
    }
    const storedId = this.readStoredClientId();
    const id = configuredId && configuredId > 0 ? configuredId : storedId;
    this.clientId.set(id);
    return id;
  }

  rememberClient(client: ClientDto): void {
    this.client.set(client);
    this.clientId.set(client.id);
    if (typeof localStorage !== 'undefined') {
      localStorage.setItem(ClientsService.clientStorageKey, String(client.id));
    }
  }

  async create(request: CreateClientRequest): Promise<ClientDto> {
    const { apiBaseUrl } = await this.config.get();
    const client = await firstValueFrom(
      this.http.post<ClientDto>(`${apiBaseUrl}/clients`, request),
    );
    this.rememberClient(client);
    return client;
  }

  async getProducts(): Promise<ProductDto[]> {
    const { apiBaseUrl } = await this.config.get();
    return firstValueFrom(this.http.get<ProductDto[]>(`${apiBaseUrl}/clients/products`));
  }

  async purchase(clientId: number, request: PurchaseRequest): Promise<PurchaseDto> {
    const { apiBaseUrl } = await this.config.get();
    return firstValueFrom(
      this.http.post<PurchaseDto>(`${apiBaseUrl}/clients/${clientId}/purchases`, request),
    );
  }

  async getPurchases(clientId: number): Promise<PurchaseDto[]> {
    const { apiBaseUrl } = await this.config.get();
    return firstValueFrom(
      this.http.get<PurchaseDto[]>(`${apiBaseUrl}/clients/${clientId}/purchases`),
    );
  }

  async createOrder(clientId: number, request: CreateOrderRequest): Promise<OrderDto> {
    const { apiBaseUrl } = await this.config.get();
    return firstValueFrom(
      this.http.post<OrderDto>(`${apiBaseUrl}/clients/${clientId}/orders`, request),
    );
  }

  async getOrders(clientId: number): Promise<OrderDto[]> {
    const { apiBaseUrl } = await this.config.get();
    return firstValueFrom(this.http.get<OrderDto[]>(`${apiBaseUrl}/clients/${clientId}/orders`));
  }

  async cancelOrder(clientId: number, orderId: number): Promise<void> {
    const { apiBaseUrl } = await this.config.get();
    await firstValueFrom(
      this.http.delete<void>(`${apiBaseUrl}/clients/${clientId}/orders/${orderId}`),
    );
  }

  private readStoredClientId(): number | null {
    if (typeof localStorage === 'undefined') return null;
    const value = Number(localStorage.getItem(ClientsService.clientStorageKey));
    return Number.isInteger(value) && value > 0 ? value : null;
  }
}
