import { Injectable } from '@angular/core';
import { BehaviorSubject, debounceTime, Observable, Subject, switchMap, tap } from 'rxjs';
import { Order, OrderSortField, OrderStatus } from '../models/order.model';
import { HttpClient, HttpParams } from '@angular/common/http';
import { PagedResult } from '../../../core/models/paged-result.model';
import { OrderStatusSignalRService } from './order-status-signal-r.service';
import { environment } from '../../../../environments/environment';

interface State {
  page: number;
  pageSize: number;
  sortBy: OrderSortField;
  sortDescending: boolean;
  orderNumber?: string;
  description?: string;
  status?: OrderStatus;
  clientId?: number;
}

@Injectable()
export class OrdersService {
  private readonly apiUrl = `${environment.apiUrl}/api/orders`;

  private _subscribedOrderIds: string[] = [];

  private _loading$: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);
  private _orders$: BehaviorSubject<Order[]> = new BehaviorSubject<Order[]>([]);
  private _total$: BehaviorSubject<number> = new BehaviorSubject<number>(0);
  private _search$: Subject<void> = new Subject<void>();

  private _state: State = {
    page: 1,
    pageSize: 10,
    sortBy: 'createdAt',
    sortDescending: true,
  };

  public get sortBy(): OrderSortField {
    return this._state.sortBy;
  }
  public get sortDescending(): boolean {
    return this._state.sortDescending;
  }

  public get orders$(): Observable<Order[]> {
    return this._orders$.asObservable();
  }

  public get total$(): Observable<number> {
    return this._total$.asObservable();
  }

  public get loading$(): Observable<boolean> {
    return this._loading$.asObservable();
  }

  public constructor(
    private _http: HttpClient,
    private _signalR: OrderStatusSignalRService,
  ) {
    this._search$
      .pipe(
        tap(() => this._loading$.next(true)),
        debounceTime(300),
        switchMap(() => this._fetchOrders()),
        tap(() => this._loading$.next(false)),
      )
      .subscribe((result: PagedResult<Order>): void => {
        this._orders$.next(result.items);
        this._total$.next(result.totalCount);
        void this._resubscribe(result.items.map((order) => order.id));
      });

    this._signalR.onStatusChanged((orderId, status, updatedAt) => {
      const orders = this._orders$.value;
      const index = orders.findIndex((o) => o.id === orderId);
      if (index === -1) return;

      const updated = [...orders];
      updated[index] = {
        ...updated[index],
        status: status as OrderStatus,
        updatedAt: new Date(updatedAt),
      };
      this._orders$.next(updated);
    });

    this._search$.next();
  }

  public get page(): number {
    return this._state.page;
  }

  public get pageSize(): number {
    return this._state.pageSize;
  }

  public set page(value: number) {
    this._set({ page: value });
  }

  public set pageSize(value: number) {
    this._set({ pageSize: value });
  }

  public refresh(): void {
    this._search$.next();
  }

  public setFilters(
    filters: Partial<Pick<State, 'orderNumber' | 'description' | 'status' | 'clientId'>>,
  ): void {
    this._set({ ...filters, page: 1 });
  }

  private _set(patch: Partial<State>): void {
    Object.assign(this._state, patch);
    this._search$.next();
  }

  private _fetchOrders(): Observable<PagedResult<Order>> {
    const { page, pageSize, sortBy, sortDescending, orderNumber, description, status, clientId } =
      this._state;

    let params = new HttpParams()
      .set('page', page)
      .set('pageSize', pageSize)
      .set('sortBy', sortBy)
      .set('sortDescending', sortDescending);

    if (orderNumber) params = params.set('orderNumber', orderNumber);
    if (description) params = params.set('description', description);
    if (status) params = params.set('status', status);
    if (clientId) params = params.set('clientId', clientId);

    return this._http.get<PagedResult<Order>>(this.apiUrl, { params });
  }

  public sort(field: OrderSortField): void {
    if (this._state.sortBy === field) {
      this._set({ sortDescending: !this._state.sortDescending });
    } else {
      this._set({ sortBy: field, sortDescending: true, page: 1 });
    }
  }

  private async _resubscribe(newOrderIds: string[]): Promise<void> {
    if (this._subscribedOrderIds.length > 0) {
      await this._signalR.unsubscribeFromOrdersAsync(this._subscribedOrderIds);
    }

    this._subscribedOrderIds = newOrderIds;
    if (newOrderIds.length > 0) {
      await this._signalR.subscribeToOrdersAsync(newOrderIds);
    }
  }
}
