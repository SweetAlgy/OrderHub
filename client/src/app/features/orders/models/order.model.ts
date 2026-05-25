export type OrderStatus = 'Created' | 'Shipped' | 'Delivered' | 'Cancelled';

export interface Order {
  id: string;
  clientId: string;
  orderNumber: string;
  description: string;
  status: OrderStatus;
  createdAt: Date;
  updatedAt: Date;
  clientName: string;
}

export type OrderSortField =
  | 'orderNumber'
  | 'description'
  | 'status'
  | 'clientName'
  | 'createdAt'
  | 'updatedAt';

