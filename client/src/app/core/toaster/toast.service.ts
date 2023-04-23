import { Injectable } from '@angular/core';
import { Observable, Subject } from "rxjs";

export enum ToastType {
  success = 'success',
  info = 'info',
  warning = 'warning',
  error = 'error',
}


export interface ToastEvent {
  title: string,
  message: string,
  type: ToastType
}

@Injectable({
  providedIn: 'root'
})
export class ToastService {
  private _toastEvents$: Subject<ToastEvent> = new Subject<ToastEvent>();
  public toastEvents$: Observable<ToastEvent>;
  constructor() {
    this.toastEvents$ = this._toastEvents$.asObservable();
  }

  public showSuccessToast(title: string, message: string) {
    const event: ToastEvent = {
      title,
      message,
      type: ToastType.success
    };
    this._toastEvents$.next(event);
  }

  public showErrorToast(title: string, message: string) {
    const event: ToastEvent = {
      title,
      message,
      type: ToastType.error
    };
    this._toastEvents$.next(event);
  }
}
