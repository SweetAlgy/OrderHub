import { Component } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { ClientService } from '../../services/client.service';
import { ToastService } from '../../services/toast.service';
import { finalize } from 'rxjs';

interface CreateClientForm {
  name: FormControl<string>;
}

@Component({
  selector: 'app-create-client-modal',
  imports: [ReactiveFormsModule],
  templateUrl: './create-client-modal.html',
})
export class CreateClientModal {
  protected form = new FormGroup<CreateClientForm>({
    name: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required, Validators.maxLength(256)],
    }),
  });

  protected isSubmitting = false;
  protected errorMessage: string | null = null;

  public constructor(
    public activeModal: NgbActiveModal,
    private _clientService: ClientService,
    private _toastService: ToastService,
  ) {}

  protected get name(): FormControl<string> {
    return this.form.controls.name;
  }

  protected dismissError(): void {
    this.errorMessage = null;
  }

  protected onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.isSubmitting = true;
    this.errorMessage = null;

    this._clientService
      .createClient(this.form.getRawValue())
      .pipe(finalize(() => (this.isSubmitting = false)))
      .subscribe({
        next: (client) => {
          this._toastService.success(`Client "${client.name}" created successfully!`);
          this.activeModal.close(client);
        },
        error: (err) => {
          this.errorMessage = err.error?.message ?? 'Something went wrong. Please try again.';
        },
        complete: () => {
          this.isSubmitting = false;
        },
      });
  }
}
