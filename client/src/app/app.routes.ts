import { Routes } from '@angular/router';
import { OrdersTable } from './features/orders/components/orders-table/orders-table';
import { PaymentsTable } from './features/payments/components/payments-table/payments-table';

export const routes: Routes = [
  { path: '', redirectTo: 'orders', pathMatch: 'full' },
  { path: 'orders', component: OrdersTable },
  { path: 'payments', component: PaymentsTable },
];
