import { Injectable } from '@angular/core';
import { NgxSpinnerService } from 'ngx-spinner';
import { Quantity } from "../../shared/enums/quantity";

@Injectable({
  providedIn: 'root'
})
export class BusyService {
  public busyRequestCount: number = Quantity.zero;

  public constructor(private spinnerService: NgxSpinnerService) {
  }

  public busy(): void {
    this.busyRequestCount++;
    this.spinnerService.show(undefined, {
      bdColor: 'rgba(255,255,255,0.5)',
      color: '#333333'
    });
  }

  public idle(): void {
    this.busyRequestCount--;
    if (this.busyRequestCount <= Quantity.zero) {
      this.busyRequestCount = Quantity.zero;
      this.spinnerService.hide();
    }
  }
}
