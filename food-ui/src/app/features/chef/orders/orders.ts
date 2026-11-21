import { CommonModule } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { Component, computed, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { ApiConfigService } from '../../../core/services/api-config.service';
import { ChefOrdersService } from '../../../core/services/commerce/chef-orders';
import { OrderDto } from '../../../shared/models/order.dto';

@Component({
  selector: 'app-chef-orders',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './orders.html',
  styleUrl: './orders.css',
})
export class ChefOrdersComponent {
  private readonly config = inject(ApiConfigService);
  readonly data = inject(ChefOrdersService);
  readonly orders = signal<OrderDto[]>([]);
  readonly loading = signal(false);
  readonly launchingId = signal<number | null>(null);
  readonly error = signal('');
  readonly pendingCount = computed(() => this.orders().filter(order => this.isPending(order)).length);
  private chefId = 0;

  constructor() {
    void this.load();
  }

  async load(): Promise<void> {
    this.loading.set(true);
    this.error.set('');
    try {
      this.chefId ||= (await this.config.get()).chefId ?? 1;
      this.orders.set(await this.data.getByChef(this.chefId));
    } catch (error) {
      this.error.set(this.message(error));
    } finally {
      this.loading.set(false);
    }
  }

  async launch(order: OrderDto): Promise<void> {
    this.launchingId.set(order.id);
    this.error.set('');
    try {
      this.chefId ||= (await this.config.get()).chefId ?? 1;
      const updated = await this.data.launch(this.chefId, order.id);
      this.orders.update(items => items.map(item => (item.id === order.id ? updated : item)));
    } catch (error) {
      this.error.set(this.message(error));
    } finally {
      this.launchingId.set(null);
    }
  }

  isPending(order: OrderDto): boolean {
    return order.status.toLowerCase() === 'pending';
  }

  statusClass(status: string): string {
    return `chef-order-${status.toLowerCase()}`;
  }

  private message(error: unknown): string {
    return error instanceof HttpErrorResponse && error.error?.detail
      ? error.error.detail
      : 'Orders could not be loaded. Please try again.';
  }
}
