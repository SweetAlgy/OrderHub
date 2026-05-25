import { Pipe, PipeTransform } from '@angular/core';
import { DailyPayment } from '../../features/payments/models/payment.model';

@Pipe({ name: 'totalAmount' })
export class TotalAmountPipe implements PipeTransform {
  public transform(payments: DailyPayment[]): number {
    return payments.reduce((sum, dailyPayment): number => sum + dailyPayment.amount, 0);
  }
}
