import { Injectable } from '@angular/core';

export interface Toast {
  message: string;
  type: 'success' | 'error' | 'warning' | 'info';
}

@Injectable({
  providedIn: 'root',
})
export class ToastService {
  private _toasts: Toast[] = [];

  public get toasts(): Toast[] {
    return this._toasts;
  }

  public success(message: string): void {
    this._toasts.push({ message, type: 'success' });
  }

  public error(message: string): void {
    this._toasts.push({ message, type: 'error' });
  }

  public warning(message: string): void {
    this._toasts.push({ message, type: 'warning' });
  }

  public info(message: string): void {
    this._toasts.push({ message, type: 'info' });
  }

  public remove(toast: Toast): void {
    this._toasts = this._toasts.slice(this._toasts.indexOf(toast), 1);
  }
}
