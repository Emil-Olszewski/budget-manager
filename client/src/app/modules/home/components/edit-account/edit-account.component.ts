import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormControl, FormGroup, Validators } from "@angular/forms";
import { Observable, take } from "rxjs";
import { AccountWithInitialBalance, Currency, UpdateAccount } from "../../models/account";

@Component({
  selector: 'app-edit-account',
  templateUrl: './edit-account.component.html',
  styleUrls: ['./edit-account.component.scss']
})
export class EditAccountComponent implements OnInit {
  private id!: number;
  public Currency = Currency;
  @Input() public account$!: Observable<AccountWithInitialBalance>;
  @Output() public save$: EventEmitter<UpdateAccount> = new EventEmitter<UpdateAccount>();
  @Output() public delete$: EventEmitter<never> = new EventEmitter<never>();
  @Output() public goBack$: EventEmitter<never> = new EventEmitter<never>();
  public form: FormGroup = new FormGroup({
    name: new FormControl('', [Validators.required, Validators.maxLength(40)]),
    currency: new FormControl('', [Validators.required]),
    initialBalance: new FormControl(),
  });

  public ngOnInit(): void {
    if (!this.account$) {
      return;
    }

    this.account$
      .pipe(take(1))
      .subscribe(x => {
        this.id = x.id;
        this.form.controls['name'].setValue(x.name);
        this.form.controls['currency'].setValue(x.currency);
        this.form.controls['initialBalance'].setValue(x.initialBalance);
      });
  }

  public submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    const account: UpdateAccount = {
      id: this.id,
      name: this.form.controls['name'].value,
      currency: +this.form.controls['currency'].value,
      initialBalance: this.form.controls['initialBalance'].value
    };
    this.save$.emit(account);
  }
}
