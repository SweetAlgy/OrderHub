import { Pipe, PipeTransform } from '@angular/core';
import { DatePipe } from '@angular/common';

@Pipe({ name: 'localDate' })
export class LocalDatePipe implements PipeTransform {
  private readonly _datePipe = new DatePipe('en-US');

  public transform(value: string | Date | null, format = 'dd.MM.yyyy HH:mm'): string | null {
    if (!value) return null;
    return this._datePipe.transform(
      value,
      format,
      Intl.DateTimeFormat().resolvedOptions().timeZone,
    );
  }
}
