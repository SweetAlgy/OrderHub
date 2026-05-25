import { ChangeDetectorRef, Component, Input, OnDestroy, OnInit } from '@angular/core';
import { Order, OrderStatus } from '../../models/order.model';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { HttpClient } from '@angular/common/http';
import { OrderStatusSignalRService } from '../../services/order-status-signal-r.service';
import { OrderStatusBadgeComponent } from '../order-status-badge/order-status-badge.component';
import { FormsModule } from '@angular/forms';
import { ToastService } from '../../../../core/services/toast.service';
import { LocalDatePipe } from '../../../../shared/pipes/local-date.pipe';
import { environment } from '../../../../../environments/environment';

@Component({
  selector: 'app-order-detail',
  imports: [OrderStatusBadgeComponent, FormsModule, LocalDatePipe],
  templateUrl: './order-detail.html',
})
export class OrderDetail implements OnInit, OnDestroy {
  @Input({ required: true })
  public order!: Order;

  private readonly _apiUrl = `${environment.apiUrl}/api/orders`;

  protected isEditing: boolean = false;
  protected isSubmitting: boolean = false;
  protected selectedStatus: OrderStatus = 'Created';

  protected readonly statuses: OrderStatus[] = ['Created', 'Shipped', 'Delivered', 'Cancelled'];

  public constructor(
    protected activeModal: NgbActiveModal,
    private _http: HttpClient,
    private _changeDetectorRef: ChangeDetectorRef,
    private _toastService: ToastService,
    private _signalRService: OrderStatusSignalRService,
  ) {}

  public ngOnInit(): void {
    this.selectedStatus = this.order.status;
    void this._subscribeToSignalRAsync();
  }

  public ngOnDestroy(): void {
    void this._signalRService.unsubscribeToOrderAsync(this.order.id);
    this._signalRService.offStatusChanged();
  }

  private async _subscribeToSignalRAsync(): Promise<void> {
    await this._signalRService.subscribeToOrderAsync(this.order.id);

    this._signalRService.onStatusChanged((orderId: string, status: string, updatedAt: string) => {
      if (orderId !== this.order.id) return;

      this.order = { ...this.order, status: status as OrderStatus, updatedAt: new Date(updatedAt) };
      this.selectedStatus = this.order.status;
      this.isEditing = false;
      this._toastService.info(`Order status updated to ${status}`);
      this._changeDetectorRef.detectChanges();
    });
  }

  protected onEdit(): void {
    this.isEditing = true;
  }

  protected onCancel(): void {
    this.isEditing = false;
    this.selectedStatus = this.order.status;
  }

  protected onSubmit(): void {
    if (this.isSubmitting) return;

    if (this.selectedStatus === this.order.status) {
      this.isEditing = false;
      return;
    }

    this.isSubmitting = true;

    this._http
      .patch<Order>(`${this._apiUrl}/${this.order.id}/status`, { status: this.selectedStatus })
      .subscribe({
        next: (order: Order) => {
          this.selectedStatus = order.status;
        },
        error: (error: any) => {
          console.error(error);
          this._toastService.error(error.error?.message ?? 'Failed to update status.');
          this._changeDetectorRef.detectChanges();
        },
        complete: () => {
          this.isSubmitting = false;
          this.isEditing = false;
        },
      });
  }
}
