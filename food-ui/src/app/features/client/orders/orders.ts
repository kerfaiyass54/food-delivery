import { CommonModule } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { Component, computed, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { ClientsService } from '../../../core/services/commerce/clients';
import { OrderDto, PurchaseDto } from '../../../shared/models/commerce.models';

@Component({
  selector: 'app-client-orders',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './orders.html',
  styleUrl: './orders.css',
})
export class ClientOrdersComponent {
  readonly data = inject(ClientsService);
  readonly orders = signal<OrderDto[]>([]);
  readonly purchases = signal<PurchaseDto[]>([]);
  readonly activeTab = signal<'orders' | 'purchases'>('orders');
  readonly loading = signal(false);
  readonly cancellingId = signal<number | null>(null);
  readonly error = signal('');
  readonly pendingCount = computed(() => this.orders().filter(order => this.isPending(order)).length);

  constructor() {
    void this.load();
  }

  async load(): Promise<void> {
    this.loading.set(true);
    this.error.set('');
    try {
      const clientId = await this.data.initializeClient();
      if (!clientId) return;
      const [orders, purchases] = await Promise.all([
        this.data.getOrders(clientId),
        this.data.getPurchases(clientId),
      ]);
      this.orders.set(orders);
      this.purchases.set(purchases);
    } catch (error) {
      this.error.set(this.message(error));
    } finally {
      this.loading.set(false);
    }
  }

  async cancel(order: OrderDto): Promise<void> {
    const clientId = this.data.clientId();
    if (!clientId) return;
    this.cancellingId.set(order.id);
    this.error.set('');
    try {
      await this.data.cancelOrder(clientId, order.id);
      this.orders.update(items =>
        items.map(item => (item.id === order.id ? { ...item, status: 'Cancelled' } : item)),
      );
    } catch (error) {
      this.error.set(this.message(error));
    } finally {
      this.cancellingId.set(null);
    }
  }

  isPending(order: OrderDto): boolean {
    return order.status.toLowerCase() === 'pending';
  }

  statusClass(status: string): string {
    return `status-${status.toLowerCase()}`;
  }

  private message(error: unknown): string {
    return error instanceof HttpErrorResponse && error.error?.detail
      ? error.error.detail
      : 'Your activity could not be loaded. Please try again.';
  }
}
