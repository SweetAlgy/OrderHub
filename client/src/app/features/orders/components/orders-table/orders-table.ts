import { FormControl, FormGroup, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { Component, OnInit } from '@angular/core';
import { AsyncPipe } from '@angular/common';
import { NgbModal, NgbModalRef, NgbPagination } from '@ng-bootstrap/ng-bootstrap';
import { debounceTime } from 'rxjs';
import { OrdersService } from '../../services/orders.service';
import { Order, OrderStatus } from '../../models/order.model';
import { OrderStatusBadgeComponent } from '../order-status-badge/order-status-badge.component';
import { CreateOrderForm } from '../create-order-form/create-order-form';
import { OrderDetail } from '../order-detail/order-detail';
import { Client } from '../../../../core/services/client.service';
import { ClientSelector } from '../../../../shared/components/client-selector/client-selector';
import { LocalDatePipe } from '../../../../shared/pipes/local-date.pipe';
import { SortIcon } from '../../../../shared/components/sort-icon/sort-icon';

@Component({
  selector: 'app-orders-table',
  providers: [OrdersService],
  imports: [
    AsyncPipe,
    NgbPagination,
    FormsModule,
    OrderStatusBadgeComponent,
    ReactiveFormsModule,
    ClientSelector,
    LocalDatePipe,
    SortIcon,
  ],
  templateUrl: './orders-table.html',
  styleUrl: './orders-table.css',
})
export class OrdersTable implements OnInit {
  protected readonly statuses: OrderStatus[] = ['Created', 'Shipped', 'Delivered', 'Cancelled'];

  protected selectedClient: Client | null = null;

  protected filterForm = new FormGroup({
    orderNumber: new FormControl(''),
    description: new FormControl(''),
    status: new FormControl(''),
  });

  public constructor(
    protected service: OrdersService,
    private _modal: NgbModal,
  ) {}

  public ngOnInit(): void {
    this.filterForm.valueChanges.pipe(debounceTime(400)).subscribe((values) => {
      this.service.setFilters({
        orderNumber: values.orderNumber || undefined,
        description: values.description || undefined,
        status: (values.status as OrderStatus) || undefined,
      });
    });
  }

  protected onClientChange(client: Client | null): void {
    this.selectedClient = client;
    const values = this.filterForm.getRawValue();
    this.service.setFilters({
      orderNumber: values.orderNumber || undefined,
      description: values.description || undefined,
      status: (values.status as OrderStatus) || undefined,
      clientId: client?.id,
    });
  }

  protected openCreateModal(): void {
    const modalRef: NgbModalRef = this._modal.open(CreateOrderForm, {
      centered: true,
    });

    modalRef.closed.subscribe(() => this.service.refresh());
  }

  protected openDetailModal(order: Order): void {
    const modalRef: NgbModalRef = this._modal.open(OrderDetail, {
      centered: true,
      backdrop: 'static',
    });
    modalRef.componentInstance.order = order;
    modalRef.closed.subscribe(() => this.service.refresh());
  }

  protected clearFilters(): void {
    this.filterForm.setValue(
      {
        orderNumber: '',
        description: '',
        status: '',
      },
      { emitEvent: false },
    );
    this.selectedClient = null;
    this.service.setFilters({});
  }
}
