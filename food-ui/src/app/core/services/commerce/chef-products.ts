import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { ApiConfigService } from '../api-config.service';
import { CookProductRequest } from '../../../shared/models/cook-product-request.model';
import { ProductDto } from '../../../shared/models/product.dto';

@Injectable({ providedIn: 'root' })
export class ChefProductsService {
  private readonly http = inject(HttpClient);
  private readonly config = inject(ApiConfigService);

  async cook(chefId: number, request: CookProductRequest): Promise<ProductDto> {
    const { apiBaseUrl } = await this.config.get();
    return firstValueFrom(
      this.http.post<ProductDto>(`${apiBaseUrl}/chefs/${chefId}/products`, request),
    );
  }

  async getByChef(chefId: number): Promise<ProductDto[]> {
    const { apiBaseUrl } = await this.config.get();
    return firstValueFrom(
      this.http.get<ProductDto[]>(`${apiBaseUrl}/chefs/${chefId}/products`),
    );
  }

  async delete(chefId: number, productId: number): Promise<void> {
    const { apiBaseUrl } = await this.config.get();
    await firstValueFrom(
      this.http.delete<void>(`${apiBaseUrl}/chefs/${chefId}/products/${productId}`),
    );
  }
}
