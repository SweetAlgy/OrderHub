import { Component, Input } from '@angular/core';
import { OrderSortField } from '../../../features/orders/models/order.model';
import { OrdersService } from '../../../features/orders/services/orders.service';

@Component({
  selector: 'app-sort-icon',
  template: `
    <span class="ms-1 text-white-50">
      @if (service.sortBy === field) {
        {{ service.sortDescending ? '↓' : '↑' }}
      } @else {
        ↕
      }
    </span>
  `,
})
export class SortIcon {
  @Input({ required: true }) public field!: OrderSortField;
  @Input({ required: true }) public service!: OrdersService;
}
