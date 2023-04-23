import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormControl, FormGroup, Validators } from "@angular/forms";
import { Observable, take } from "rxjs";
import { Account } from "../../models/account";

@Component({
  selector: 'app-edit-account',
  templateUrl: './edit-account.component.html',
  styleUrls: ['./edit-account.component.scss']
})
export class EditAccountComponent implements OnInit {
  private id!: number;
  @Input() public account$!: Observable<Account>;
  @Output() public save$: EventEmitter<Account> = new EventEmitter<Account>();
  @Output() public delete$: EventEmitter<never> = new EventEmitter<never>();
  @Output() public goBack$: EventEmitter<never> = new EventEmitter<never>();
  public form: FormGroup = new FormGroup({
    name: new FormControl('', [Validators.required, Validators.maxLength(40)])
  });

  public ngOnInit(): void {
    if (!this.account$) {
      return;
    }

    this.account$
      .pipe(take(1))
      .subscribe(x => {
        this.form.controls['name'].setValue(x.name);
        this.id = x.id;
      });
  }

  public submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    const account: Account = {
      id: this.id,
      name: this.form.controls['name'].value
    };
    this.save$.emit(account);
  }
}
