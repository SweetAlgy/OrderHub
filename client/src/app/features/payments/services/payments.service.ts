import { Injectable } from '@angular/core';
import { BehaviorSubject, finalize, Observable } from 'rxjs';
import { DailyPayment } from '../models/payment.model';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from '../../../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class PaymentsService {
  private readonly _apiUrl = `${environment.apiUrl}/api/clients`;

  private _payments$ = new BehaviorSubject<DailyPayment[]>([]);
  private _isLoading$ = new BehaviorSubject<boolean>(false);
  private _error$ = new BehaviorSubject<string | null>(null);

  public get payments$(): Observable<DailyPayment[]> {
    return this._payments$.asObservable();
  }

  public get error$(): Observable<string | null> {
    return this._error$.asObservable();
  }

  public get isLoading(): Observable<boolean> {
    return this._isLoading$.asObservable();
  }
  public constructor(private _http: HttpClient) {}

  public load(clientId: number, from: string, to: string): void {
    this._isLoading$.next(true);
    this._error$.next(null);
    this._payments$.next([]);

    const params = new HttpParams().set('from', from).set('to', to);

    this._http
      .get<DailyPayment[]>(`${this._apiUrl}/${clientId}/payments/daily-summary`, { params })
      .pipe(finalize(() => this._isLoading$.next(false)))
      .subscribe({
        next: (payments: DailyPayment[]): void => this._payments$.next(payments),
        error: (err) => this._error$.next(err.error?.message ?? 'Failed to load payments.'),
      });
  }
}
