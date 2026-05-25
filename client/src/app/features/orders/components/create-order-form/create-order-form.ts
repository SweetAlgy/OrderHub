import { ChangeDetectorRef, Component } from '@angular/core';
import { FormControl, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Order } from '../../models/order.model';
import { HttpClient } from '@angular/common/http';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { ToastService } from '../../../../core/services/toast.service';
import { environment } from '../../../../../environments/environment';

interface CreateOrderFormGroup {
  clientId: FormControl<number | null>;
  orderNumber: FormControl<string>;
  description: FormControl<string>;
}

@Component({
  selector: 'app-create-order-form',
  imports: [ReactiveFormsModule],
  templateUrl: './create-order-form.html',
})
export class CreateOrderForm {
  private readonly _apiUrl = `${environment.apiUrl}/api/orders`;

  protected form: FormGroup<CreateOrderFormGroup> = new FormGroup<CreateOrderFormGroup>({
    clientId: new FormControl<number | null>(null, [Validators.required, Validators.min(1)]),
    orderNumber: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required, Validators.maxLength(64)],
    }),
    description: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required, Validators.maxLength(256)],
    }),
  });

  protected isSubmitting: boolean = false;
  protected errorMessage: string | null = null;

  public constructor(
    private _http: HttpClient,
    private _changeDetectorRef: ChangeDetectorRef,
    private _toastService: ToastService,
    public activeModal: NgbActiveModal,
  ) {}

  protected get clientId(): FormControl<number | null> {
    return this.form.controls.clientId;
  }

  protected get orderNumber(): FormControl<string> {
    return this.form.controls.orderNumber;
  }

  protected get description(): FormControl<string> {
    return this.form.controls.description;
  }

  protected onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.isSubmitting = true;
    this.errorMessage = null;

    this._http.post<Order>(this._apiUrl, this.form.getRawValue()).subscribe({
      next: (order: Order) => {
        this._toastService.success(`Order ${order.orderNumber} created successfully!`);
        this.activeModal.close(order);
        this.form.reset();
        this.isSubmitting = false;
      },
      error: (error: any) => {
        console.error(error);
        this.errorMessage =
          error.error?.message ?? error.message ?? 'Something went wrong. Please try again.';
        this.isSubmitting = false;
        this._changeDetectorRef.detectChanges();
      },
    });
  }

  protected dismissError(): void {
    this.errorMessage = null;
    this._changeDetectorRef.detectChanges();
  }
}
