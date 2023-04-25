import { Pipe, PipeTransform } from '@angular/core';
import { Currency } from "../../modules/home/models/account";

@Pipe({
  name: 'CurrencyCode'
})
export class CurrencyCodePipe implements PipeTransform {
  transform(value: Currency): string {
    switch (value) {
      case Currency.pln:
        return 'PLN';
      case Currency.eur:
        return 'EUR';
      case Currency.usd:
        return 'USD';
      default:
        return '???';
    }
  }
}
