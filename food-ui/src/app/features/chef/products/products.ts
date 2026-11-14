import { CommonModule } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { ApiConfigService } from '../../../core/services/api-config.service';
import { ChefProductsService } from '../../../core/services/commerce/chef-products';
import { CookProductRequest, ProductDto } from '../../../shared/models/commerce.models';

@Component({
  selector: 'app-chef-products',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './products.html',
  styleUrl: './products.css',
})
export class ChefProductsComponent {
  private readonly config = inject(ApiConfigService);
  readonly data = inject(ChefProductsService);
  readonly products = signal<ProductDto[]>([]);
  readonly loading = signal(false);
  readonly cooking = signal(false);
  readonly deletingId = signal<number | null>(null);
  readonly error = signal('');
  readonly success = signal('');
  readonly draft: CookProductRequest = { name: '', type: '', quantity: 1 };
  private chefId = 0;

  constructor() {
    void this.load();
  }

  async load(): Promise<void> {
    this.loading.set(true);
    this.error.set('');
    try {
      this.chefId ||= (await this.config.get()).chefId ?? 1;
      this.products.set(await this.data.getByChef(this.chefId));
    } catch (error) {
      this.error.set(this.message(error, 'Your products could not be loaded.'));
    } finally {
      this.loading.set(false);
    }
  }

  async cook(): Promise<void> {
    this.cooking.set(true);
    this.error.set('');
    this.success.set('');
    try {
      this.chefId ||= (await this.config.get()).chefId ?? 1;
      const product = await this.data.cook(this.chefId, this.draft);
      this.products.update(items => {
        const existing = items.findIndex(item => item.id === product.id);
        return existing < 0
          ? [product, ...items]
          : items.map((item, index) => (index === existing ? product : item));
      });
      this.success.set(`${product.name} is ready. Stock updated to ${product.stock}.`);
      this.draft.name = '';
      this.draft.type = '';
      this.draft.quantity = 1;
    } catch (error) {
      this.error.set(this.message(error, 'The product could not be added.'));
    } finally {
      this.cooking.set(false);
    }
  }

  async remove(product: ProductDto): Promise<void> {
    this.deletingId.set(product.id);
    this.error.set('');
    this.success.set('');
    try {
      this.chefId ||= (await this.config.get()).chefId ?? 1;
      await this.data.delete(this.chefId, product.id);
      this.products.update(items => items.filter(item => item.id !== product.id));
      this.success.set(`${product.name} was removed from your menu.`);
    } catch (error) {
      this.error.set(this.message(error, 'This product could not be removed.'));
    } finally {
      this.deletingId.set(null);
    }
  }

  private message(error: unknown, fallback: string): string {
    return error instanceof HttpErrorResponse && error.error?.detail ? error.error.detail : fallback;
  }
}
