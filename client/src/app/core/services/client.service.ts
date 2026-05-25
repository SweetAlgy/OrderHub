import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { HttpClient, HttpParams } from '@angular/common/http';
import { PagedResult } from '../models/paged-result.model';
import { environment } from '../../../environments/environment';

export interface Client {
  id: number;
  name: string;
}

export interface CreateClientDto {
  name: string;
}

@Injectable({
  providedIn: 'root',
})
export class ClientService {
  private readonly _apiUrl = `${environment.apiUrl}/api/clients`;

  private _clients$ = new BehaviorSubject<Client[]>([]);
  private _isLoading$ = new BehaviorSubject<boolean>(false);
  private _hasMore$ = new BehaviorSubject<boolean>(true);

  private _page = 1;
  private readonly _pageSize = 20;

  public constructor(private _http: HttpClient) {
    this.loadNextPage();
  }

  public get clients$(): Observable<Client[]> {
    return this._clients$.asObservable();
  }

  public get isLoading$(): Observable<boolean> {
    return this._isLoading$.asObservable();
  }

  public get hasMore$(): Observable<boolean> {
    return this._hasMore$.asObservable();
  }

  public createClient(dto: { name: string }): Observable<Client> {
    return this._http.post<Client>(this._apiUrl, dto).pipe(tap(() => {
      this._page = 1;
      this._clients$.next([]);
      this._hasMore$.next(true);
      this._isLoading$.next(false);
      this.loadNextPage();
    }));
  }

  public loadNextPage(): void {
    if (this._isLoading$.value || !this._hasMore$.value) return;

    this._isLoading$.next(true);

    const params = new HttpParams().set('page', this._page).set('pageSize', this._pageSize);

    this._http.get<PagedResult<Client>>(this._apiUrl, { params }).subscribe({
      next: (result: PagedResult<Client>) => {
        this._clients$.next([...this._clients$.value, ...result.items]);
        this._hasMore$.next(result.hasNextPage);
        this._isLoading$.next(false);
        this._page++;
      },
      error: () => this._isLoading$.next(false),
    });
  }
}
