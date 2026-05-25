import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder, HubConnectionState } from '@microsoft/signalr';
import { environment } from '../../../../environments/environment';

const SUBSCRIBE_TO_ORDER = 'SubscribeToOrder';
const UNSUBSCRIBE_FROM_ORDER = 'UnsubscribeFromOrder';
const ORDER_STATUS_CHANGED = 'OrderStatusChanged';

@Injectable({
  providedIn: 'root',
})
export class OrderStatusSignalRService {
  private _connection: HubConnection;

  public constructor() {
    this._connection = new HubConnectionBuilder()
      .withUrl(`${environment.apiUrl}/hubs/orders`)
      .withAutomaticReconnect()
      .build();
  }

  public async startAsync(): Promise<void> {
    if (this._connection.state === HubConnectionState.Disconnected) {
      await this._connection.start();
    }
  }

  public async subscribeToOrderAsync(orderId: string): Promise<void> {
    await this.startAsync();
    await this._connection.invoke(SUBSCRIBE_TO_ORDER, orderId);
  }

  public async unsubscribeToOrderAsync(orderId: string): Promise<void> {
    await this._connection.invoke(UNSUBSCRIBE_FROM_ORDER, orderId);
  }

  public async subscribeToOrdersAsync(orderIds: string[]): Promise<void> {
    await this.startAsync();
    await this._connection.invoke('SubscribeToOrders', orderIds);
  }

  public async unsubscribeFromOrdersAsync(orderIds: string[]): Promise<void> {
    if (this._connection.state !== HubConnectionState.Connected) return;
    await this._connection.invoke('UnsubscribeFromOrders', orderIds);
  }

  public onStatusChanged(
    callback: (orderId: string, status: string, updatedAt: string) => void,
  ): void {
    this._connection.on(
      ORDER_STATUS_CHANGED,
      (data: { orderId: string; status: string; updatedAt: string }) =>
        callback(data.orderId, data.status, data.updatedAt),
    );
  }

  public offStatusChanged(): void {
    this._connection.off(ORDER_STATUS_CHANGED);
  }
}
