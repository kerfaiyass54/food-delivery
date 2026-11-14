import { Component, computed, input, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Order } from '../../models/dashboard.model';
import { IconComponent } from '../icon/icon';

@Component({
  selector: 'app-orders-table',
  standalone: true,
  imports: [FormsModule, IconComponent],
  templateUrl: './orders-table.html',
  styleUrl: './orders-table.css',
})
export class OrdersTableComponent {
  orders = input.required<Order[]>();
  query = '';
  statusFilter = 'All orders';
  pageSize = 5;
  readonly Math = Math;
  private sortKey = signal<keyof Order>('time');
  private sortDirection = signal(1);
  currentPage = signal(1);
  filteredOrders = computed(() => {
    const q = this.query.trim().toLowerCase();
    return this.orders()
      .filter(
        o =>
          (this.statusFilter === 'All orders' || o.status === this.statusFilter) &&
          (!q || `${o.id} ${o.customer} ${o.items} ${o.status}`.toLowerCase().includes(q))
      )
      .sort(
        (a, b) =>
          String(a[this.sortKey()]).localeCompare(String(b[this.sortKey()]), undefined, {
            numeric: true,
          }) * this.sortDirection()
      );
  });
  pageCount = computed(() => Math.max(1, Math.ceil(this.filteredOrders().length / this.pageSize)));
  pages = computed(() => Array.from({ length: this.pageCount() }, (_, i) => i + 1));
  paginatedOrders = computed(() => {
    const max = this.pageCount();
    if (this.currentPage() > max) this.currentPage.set(max);
    const start = (this.currentPage() - 1) * this.pageSize;
    return this.filteredOrders().slice(start, start + this.pageSize);
  });
  sortBy(key: keyof Order) {
    if (this.sortKey() === key) this.sortDirection.update(v => v * -1);
    else {
      this.sortKey.set(key);
      this.sortDirection.set(1);
    }
  }
  goToPage(page: number) {
    this.currentPage.set(Math.max(1, Math.min(page, this.pageCount())));
  }
}
