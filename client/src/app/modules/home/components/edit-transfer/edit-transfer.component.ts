import { Account, Currency } from './../../models/account';
import {
  ChangeDetectionStrategy,
  ChangeDetectorRef,
  Component,
  EventEmitter,
  Input,
  OnDestroy,
  OnInit,
  Output, ViewChildren
} from '@angular/core';
import { combineLatestWith, Observable, ReplaySubject, take, takeUntil } from 'rxjs';
import { TransferTransaction, TransferTransactionModel } from '../../models/transfer-transaction';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { CurrencyCodePipe } from "../../../../shared/pipes/currency-code.pipe";
import { CurrencyCodeDirective } from "../../../../shared/directives/currency-code.directive";
import { CurrencyPipe } from "@angular/common";

@Component({
  selector: 'app-edit-transfer',
  templateUrl: './edit-transfer.component.html',
  styleUrls: ['./edit-transfer.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class EditTransferComponent implements OnInit, OnDestroy {
  private id!: number;
  private accounts: Account[] = [];
  private notifier$ = new ReplaySubject<never>();
  private refreshLock: boolean = false;
  @ViewChildren(CurrencyCodeDirective) directives!: CurrencyCodeDirective[];
  @Input() public transfer$!: Observable<TransferTransaction>;
  @Input() public accounts$!: Observable<Account[]>;
  @Output() public save$: EventEmitter<TransferTransactionModel> = new EventEmitter<TransferTransactionModel>();
  @Output() public delete$: EventEmitter<never> = new EventEmitter<never>();
  @Output() public goBack$: EventEmitter<never> = new EventEmitter<never>()
  public calculatedAmount$: ReplaySubject<number | null> = new ReplaySubject<number | null>();
  public form: FormGroup = new FormGroup({
    accountFromId: new FormControl('', [Validators.required]),
    accountToId : new FormControl('', [Validators.required]),
    amount: new FormControl('', [Validators.required, Validators.min(0.01)]),
    currencyConversionRate: new FormControl({value: '', disabled: true}, [Validators.required, Validators.min(0.0001)]),
    date: new FormControl(new Date().toJSON().split('T')[0], [Validators.required]),
  });

  constructor(private ref: ChangeDetectorRef) {
  }

  public get AccountFromCurrency(): Currency {
    return this.accounts.find(x => x.id === +this.form.controls['accountFromId'].value)?.currency ?? Currency.notDefined;
  }

  public ngOnInit(): void {
    this.accounts$.pipe(takeUntil(this.notifier$))
      .subscribe(x => this.accounts = x);

      this.transfer$.pipe(take(1), combineLatestWith(this.accounts$))
      .subscribe(x => {
        this.refreshLock = true;
        this.form.controls['accountFromId'].setValue(x[0].accountFrom.id);
        this.form.controls['accountFromId'].disable();
        this.form.controls['accountToId'].setValue(x[0].accountTo.id);
        this.form.controls['accountToId'].disable();
        this.form.controls['amount'].setValue(-1 * x[0].inputTransaction.amount);
        this.form.controls['currencyConversionRate'].setValue(x[0].currencyConversionRate);
        this.form.controls['date'].setValue(new Date(x[0].date).toJSON().split('T')[0]);
        this.id = x[0].id;
        this.refreshLock = false;
        if (x[0].currencyConversionRate) {
          this.enableCurrencyConversionRate();
        }
        const account = x[1].find(account => account.id === x[0].accountFrom.id)!.currency;
        // though timeout value is 0, it helps to properly initialize the directive
        setTimeout(() => {
          this.directives.forEach(x => {
            x.initWithCurrency(account)
         });
       }, 0);
      });

      this.form.controls['amount'].valueChanges
        .pipe(takeUntil(this.notifier$))
        .subscribe(x => {
          const rate: number =  +this.form.controls['currencyConversionRate'].value;
          if (rate > 0) {
            this.calculatedAmount$.next(x * rate);
          } else {
            this.calculatedAmount$.next(x);
          }
      });

      this.form.controls['accountToId'].valueChanges
        .pipe(takeUntil(this.notifier$))
        .subscribe(value => {
          this.sameAccounts();
          this.refresh();
        });

      this.form.controls['accountFromId'].valueChanges
        .pipe(takeUntil(this.notifier$))
        .subscribe(value => {
          this.sameAccounts();
          this.refresh()
          this.updateCurrencyDirective();
        });

      this.form.controls['amount'].valueChanges
        .pipe(takeUntil(this.notifier$))
        .subscribe(() => this.refresh());

      this.form.controls['currencyConversionRate'].valueChanges
        .pipe(takeUntil(this.notifier$))
        .subscribe(() => this.refresh());
  }

  public ngOnDestroy() {
    this.notifier$.next(null as never);
    this.notifier$.complete();
  }

  public submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const rate: number =  +this.form.controls['currencyConversionRate'].value;
    const model: TransferTransactionModel = {
      id: this.id,
      accountFromId: this.form.controls['accountFromId'].value,
      accountToId: this.form.controls['accountToId'].value,
      amount: this.form.controls['amount'].value,
      currencyConversionRate: rate > 0 ? rate : null,
      date: this.form.controls['date'].value,
    }
    this.save$.emit(model);
  }

  public getCurrencyNameFromAccountTo(): string {
      const accountToId = this.form.controls['accountToId'].value;
      if (!accountToId) {
       return '';
      }

    const accountTo = this.accounts.find(x => x.id === +accountToId);
    return new CurrencyCodePipe().transform(accountTo!.currency);
  }

  private refresh(): void
  {
    if (this.refreshLock) {
      return;
    }

    this.refreshLock = true;

    this.refreshConversion();
    this.setConversionRateState()

    this.refreshLock = false;
  }

  private refreshConversion() {
    const accountFromId = this.form.controls['accountFromId'].value;
    const accountFrom = this.accounts.find(x => x.id === +accountFromId);

    const accountToId = this.form.controls['accountToId'].value;
    const accountTo = this.accounts.find(x => x.id === +accountToId);

    const amount = +this.form.controls['amount'].value;
    if (!accountFrom || !accountTo || !amount) {
      return;
    }

    if (accountFrom.currency !== accountTo.currency && this.form.controls['amount'].valid) {
      const conversionRate = +this.form.controls['currencyConversionRate'].value ?? 1;
      this.calculatedAmount$.next(amount * conversionRate);
    } else {
      this.calculatedAmount$.next(null);
    }
  }

  private setConversionRateState() {
    if (this.areBothAccountsSelectedAndHaveDifferentCurrency()) {
      this.enableCurrencyConversionRate();
    } else {
      this.disableCurrencyConversionRate();
    }
  }

  private areBothAccountsSelectedAndHaveDifferentCurrency(): boolean {
    const accountFromId = this.form.controls['accountFromId'].value;
    const accountToId = this.form.controls['accountToId'].value;

    if (!accountFromId || !accountToId || this.accounts.length === 0) {
      return false;
    }

    const accountFrom = this.accounts.find(x => x.id === +accountFromId);
    const accountTo = this.accounts.find(x => x.id === +accountToId);
    return accountFrom!.currency !== accountTo!.currency;
  }

  public updateCurrencyDirective(): void {
    const accountFromId = this.form.controls['accountFromId'].value;
    const accountFrom = this.accounts.find(x => x.id === +accountFromId);

    this.directives.forEach(x => x.currencyChanged(accountFrom!.currency));
  }

  public enableCurrencyConversionRate(): void {
    this.form.controls['currencyConversionRate'].enable();
  }

  public disableCurrencyConversionRate(): void {
    this.form.controls['currencyConversionRate'].reset();
    this.form.controls['currencyConversionRate'].disable();
  }

  public sameAccounts(): void {
    const accountFromId = this.form.controls['accountFromId'].value;
    const accountToId = this.form.controls['accountToId'].value;
    if (accountFromId === accountToId) {
      this.form.controls['accountFromId'].setErrors({sameValue: true});
      this.form.controls['accountToId'].setErrors({sameValue: true});
    } else {
      this.form.controls['accountFromId'].setErrors(null);
      this.form.controls['accountToId'].setErrors(null);
    }
  }
}

