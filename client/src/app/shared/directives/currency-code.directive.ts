import { Directive, ElementRef, HostListener, Input } from '@angular/core';
import { CurrencyPipe } from "@angular/common";
import { Currency } from "../../modules/home/models/account";
import { CurrencyCodePipe } from "../pipes/currency-code.pipe";

@Directive({
  selector: '[appCurrencyCode]'
})
export class CurrencyCodeDirective {
  private amount!: number;
  private currencyPipe: CurrencyPipe = new CurrencyPipe('en-US');
  private currencyCodePipe: CurrencyCodePipe = new CurrencyCodePipe();

  @Input() appCurrencyCode: Currency = Currency.notDefined;
  constructor(private el: ElementRef) { }

  @HostListener('blur') onBlur() {
    this.bindAmount();
    this.formatCurrencyCode(this.appCurrencyCode);
  }

  @HostListener('focus') onFocus() {
    this.el.nativeElement.value = this.amount ?? '';
  }

  public currencyChanged(currency: Currency) {
    this.formatCurrencyCode(currency);
  }

  public initWithCurrency(currency: Currency) {
    console.log('initWithCurrency')
    this.bindAmount();
    this.formatCurrencyCode(currency);
  }

  private formatCurrencyCode(currency: Currency) {
    this.el.nativeElement.value = this.currencyPipe
      .transform(+this.amount, this.currencyCodePipe.transform(currency), 'symbol-narrow', '1.2-2', 'en-GB') ?? '';
  }

  private bindAmount(): void {
    this.amount = +this.el.nativeElement.value;
    if (isNaN(this.amount)) {
      this.amount = 0;
      this.el.nativeElement.value = 0;
    }
  }
}
