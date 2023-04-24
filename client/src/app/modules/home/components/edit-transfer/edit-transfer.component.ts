import { Account } from './../../models/account';
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { Observable, take } from 'rxjs';
import { TransferTransaction, TransferTransactionModel } from '../../models/transfer-transaction';
import { FormControl, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'app-edit-transfer',
  templateUrl: './edit-transfer.component.html',
  styleUrls: ['./edit-transfer.component.scss']
})
export class EditTransferComponent implements OnInit {
  private id!: number;
  @Input() public transfer$!: Observable<TransferTransaction>;
  @Input() public accounts$!: Observable<Account[]>;
  @Output() public save$: EventEmitter<TransferTransactionModel> = new EventEmitter<TransferTransactionModel>();
  @Output() public delete$: EventEmitter<never> = new EventEmitter<never>();
  @Output() public goBack$: EventEmitter<never> = new EventEmitter<never>()
  public form: FormGroup = new FormGroup({
    accountFromId: new FormControl('', [Validators.required]),
    accountToId : new FormControl('', [Validators.required]),
    amount: new FormControl('', [Validators.required, Validators.min(0.01)]),
    currencyConversionRate: new FormControl('', [Validators.min(0.0001)]),
    date: new FormControl(new Date().toJSON().split('T')[0], [Validators.required]),
  });

  public ngOnInit(): void {
      this.transfer$.pipe(take(1))
      .subscribe(x => {
        this.form.controls['accountFromId'].setValue(x.accountFrom.id);
        this.form.controls['accountToId'].setValue(x.accountTo.id);
        this.form.controls['amount'].setValue(-1 * x.inputTransaction.amount);
        this.form.controls['currencyConversionRate'].setValue(x.currencyConversionRate);
        this.form.controls['date'].setValue(new Date(x.date).toJSON().split('T')[0]);
        this.id = x.id;
      });
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
}
