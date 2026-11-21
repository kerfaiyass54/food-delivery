import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { ApiConfigService } from '../api-config.service';
import { OrderDto } from '../../../shared/models/order.dto';

@Injectable({ providedIn: 'root' })
export class ChefOrdersService {
  private readonly http = inject(HttpClient);
  private readonly config = inject(ApiConfigService);

  async getByChef(chefId: number): Promise<OrderDto[]> {
    const { apiBaseUrl } = await this.config.get();
    return firstValueFrom(this.http.get<OrderDto[]>(`${apiBaseUrl}/chefs/${chefId}/orders`));
  }

  async launch(chefId: number, orderId: number): Promise<OrderDto> {
    const { apiBaseUrl } = await this.config.get();
    return firstValueFrom(
      this.http.put<OrderDto>(`${apiBaseUrl}/chefs/${chefId}/orders/${orderId}/launch`, null),
    );
  }
}
