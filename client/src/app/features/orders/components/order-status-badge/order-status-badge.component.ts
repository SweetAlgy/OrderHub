import { Component, Input } from '@angular/core';
import { OrderStatus } from '../../models/order.model';

@Component({
  selector: 'app-order-status-badge',
  template: `<span class="badge" [class]="badgeClass">{{ status }}</span>`,
})
export class OrderStatusBadgeComponent {
  @Input({ required: true }) status!: OrderStatus;

  get badgeClass(): string {
    const map: Record<OrderStatus, string> = {
      Created: 'bg-secondary',
      Shipped: 'bg-primary',
      Delivered: 'bg-success',
      Cancelled: 'bg-danger',
    };
    return map[this.status];
  }
}
