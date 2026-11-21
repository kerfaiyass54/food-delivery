import { CommonModule } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { ClientsService } from '../../../core/services/commerce/clients';
import { ProductDto } from '../../../shared/models/product.dto';

@Component({
  selector: 'app-client-shop',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './shop.html',
  styleUrl: './shop.css',
})
export class ClientShopComponent {
  readonly data = inject(ClientsService);
  readonly products = signal<ProductDto[]>([]);
  readonly loading = signal(false);
  readonly actionProductId = signal<number | null>(null);
  readonly error = signal('');
  readonly success = signal('');
  readonly quantities = signal<Record<number, number>>({});
  orderTime = '19:00';

  constructor() {
    void this.load();
  }

  async load(): Promise<void> {
    this.loading.set(true);
    this.error.set('');
    try {
      await this.data.initializeClient();
      this.products.set(await this.data.getProducts());
    } catch (error) {
      this.error.set(this.message(error, 'The menu could not be loaded.'));
    } finally {
      this.loading.set(false);
    }
  }

  quantity(productId: number): number {
    return this.quantities()[productId] ?? 1;
  }

  setQuantity(productId: number, quantity: number): void {
    this.quantities.update(values => ({ ...values, [productId]: Math.max(1, Number(quantity) || 1) }));
  }

  async buy(product: ProductDto): Promise<void> {
    const clientId = this.data.clientId();
    const quantity = this.quantity(product.id);
    if (!clientId) {
      this.error.set('Create your client profile before making a purchase.');
      return;
    }
    if (product.price === null || product.price <= 0) {
      this.error.set('This product has no price yet. Ask the chef to set its price before purchasing.');
      return;
    }
    if (quantity > product.stock) {
      this.error.set(`Only ${product.stock} unit(s) are currently available.`);
      return;
    }

    this.beginAction(product.id);
    try {
      await this.data.purchase(clientId, { productId: product.id, quantity });
      this.success.set(`${product.name} has been added to your purchases.`);
      await this.refreshProducts();
    } catch (error) {
      this.error.set(this.message(error, 'The purchase could not be completed.'));
    } finally {
      this.actionProductId.set(null);
    }
  }

  async order(product: ProductDto): Promise<void> {
    const clientId = this.data.clientId();
    const quantity = this.quantity(product.id);
    if (!clientId) {
      this.error.set('Create your client profile before placing an order.');
      return;
    }
    if (quantity > product.stock) {
      this.error.set(`Only ${product.stock} unit(s) are currently available.`);
      return;
    }

    this.beginAction(product.id);
    try {
      await this.data.createOrder(clientId, {
        products: [{ productId: product.id, quantity }],
        time: `${this.orderTime}:00`,
      });
      this.success.set(`Your order for ${product.name} is on its way to the chef.`);
      await this.refreshProducts();
    } catch (error) {
      this.error.set(this.message(error, 'The order could not be placed.'));
    } finally {
      this.actionProductId.set(null);
    }
  }

  private beginAction(productId: number): void {
    this.error.set('');
    this.success.set('');
    this.actionProductId.set(productId);
  }

  private async refreshProducts(): Promise<void> {
    this.products.set(await this.data.getProducts());
  }

  private message(error: unknown, fallback: string): string {
    return error instanceof HttpErrorResponse && error.error?.detail ? error.error.detail : fallback;
  }
}
