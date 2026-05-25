import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { PaymentsService } from '../../services/payments.service';
import { Client, ClientService } from '../../../../core/services/client.service';
import { AsyncPipe, CurrencyPipe } from '@angular/common';
import { TotalAmountPipe } from '../../../../shared/pipes/total-amount.pipe';
import { Observable } from 'rxjs';
import { BaseChartDirective } from 'ng2-charts';
import { ChartData, ChartOptions } from 'chart.js';
import { DailyPayment } from '../../models/payment.model';
import { ClientSelector } from '../../../../shared/components/client-selector/client-selector';
import { LocalDatePipe } from '../../../../shared/pipes/local-date.pipe';

interface PaymentsForm {
  from: FormControl<string>;
  to: FormControl<string>;
}

@Component({
  selector: 'app-payments-table',
  imports: [
    CurrencyPipe,
    TotalAmountPipe,
    AsyncPipe,
    ReactiveFormsModule,
    BaseChartDirective,
    ClientSelector,
    LocalDatePipe,
  ],
  templateUrl: './payments-table.html',
})
export class PaymentsTable implements OnInit {
  protected form = new FormGroup<PaymentsForm>({
    from: new FormControl('', { nonNullable: true, validators: [Validators.required] }),
    to: new FormControl('', { nonNullable: true, validators: [Validators.required] }),
  });

  protected selectedClient: Client | null = null;

  protected chartData: ChartData<'bar'> = {
    labels: [],
    datasets: [
      {
        data: [],
        label: 'Amount',
        backgroundColor: 'rgba(54, 162, 235, 0.6)',
        borderColor: 'rgba(54, 162, 235, 1)',
        borderWidth: 1,
        borderRadius: 4,
      },
    ],
  };

  protected chartOptions: ChartOptions<'bar'> = {
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
      legend: { display: false },
      tooltip: {
        callbacks: {
          label: (ctx) => `\$${(ctx.parsed.y ?? 0).toFixed(2)}`,
        },
      },
    },
    scales: {
      x: {
        grid: { display: false },
        ticks: { maxRotation: 45 },
      },
      y: {
        beginAtZero: true,
        ticks: {
          callback: (value) => `$${value}`,
        },
      },
    },
  };

  public constructor(
    protected paymentsService: PaymentsService,
    protected clientService: ClientService,
  ) {}

  public get isLoading(): Observable<boolean> {
    return this.paymentsService.isLoading;
  }

  public ngOnInit(): void {
    const to = new Date();
    const from = new Date();
    from.setDate(to.getDate() - 10);

    this.form.patchValue({
      from: from.toISOString().split('T')[0],
      to: to.toISOString().split('T')[0],
    });

    this.paymentsService.payments$.subscribe((payments) => {
      this._updateChart(payments);
    });
  }

  protected onLoad(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    if (!this.selectedClient) return;

    const { from, to } = this.form.getRawValue();
    this.paymentsService.load(this.selectedClient.id, from, to);
  }

  protected onClientChange(client: Client | null): void {
    this.selectedClient = client;
    console.log(this.selectedClient);
  }

  private _updateChart(payments: DailyPayment[]): void {
    this.chartData = {
      labels: payments.map((p) => {
        const date = new Date(p.paymentDate);
        return date.toLocaleDateString('en-GB', { day: '2-digit', month: 'short' });
      }),
      datasets: [
        {
          data: payments.map((p) => p.amount),
          label: 'Amount',
          backgroundColor: payments.map((p) =>
            p.amount > 0 ? 'rgba(54, 162, 235, 0.6)' : 'rgba(200, 200, 200, 0.3)',
          ),
          borderColor: payments.map((p) =>
            p.amount > 0 ? 'rgba(54, 162, 235, 1)' : 'rgba(200, 200, 200, 0.5)',
          ),
          borderWidth: 1,
          borderRadius: 4,
        },
      ],
    };
  }
}
