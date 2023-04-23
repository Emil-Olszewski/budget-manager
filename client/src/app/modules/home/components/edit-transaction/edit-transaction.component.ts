import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { BehaviorSubject, combineLatestWith, forkJoin, mergeMap, Observable, take } from "rxjs";
import { Transaction, TransactionType, UpdateTransaction } from "../../models/transaction";
import { FormControl, FormGroup, Validators } from "@angular/forms";
import { Account } from "../../models/account";
import { Tag } from "../../models/tag";

@Component({
  selector: 'app-edit-transaction',
  templateUrl: './edit-transaction.component.html',
  styleUrls: ['./edit-transaction.component.scss']
})
export class EditTransactionComponent implements OnInit {
  private id!: number;
  @Input() public transaction$!: Observable<Transaction>;
  @Input() public accounts$!: Observable<Account[]>;
  @Input() public tags$!: Observable<Tag[]>
  @Output() public save$: EventEmitter<UpdateTransaction> = new EventEmitter<UpdateTransaction>();
  @Output() public delete$: EventEmitter<never> = new EventEmitter<never>();
  @Output() public goBack$: EventEmitter<never> = new EventEmitter<never>()
  public transactionTags$: BehaviorSubject<Tag[]> = new BehaviorSubject<Tag[]>([]);
  public form: FormGroup = new FormGroup({
    account: new FormControl('', [Validators.required]),
    name: new FormControl('', [Validators.required, Validators.maxLength(40)]),
    amount: new FormControl('', [Validators.required]),
    date: new FormControl(new Date().toJSON().split('T')[0], [Validators.required]),
  });

  public ngOnInit(): void {
    this.transaction$
      .pipe(take(1), combineLatestWith(this.tags$))
      .subscribe(x => {
        this.form.controls['account'].setValue(x[0].accountId);
        this.form.controls['name'].setValue(x[0].name);
        this.form.controls['amount'].setValue(x[0].amount);
        this.form.controls['date'].setValue((new Date(x[0].date).toJSON().split('T')[0]));
        this.transactionTags$.next(x[1].filter(y => x[0].tags.map(z => z.id).includes(y.id)));
        this.id = x[0].id;
      });
  }

  public submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    const transaction: UpdateTransaction = {
      id: this.id,
      name: this.form.controls['name'].value,
      type: this.form.controls['amount'].value > 0 ? TransactionType.income : TransactionType.expense,
      amount: this.form.controls['amount'].value,
      date: this.form.controls['date'].value,
      accountId: this.form.controls['account'].value,
      tagIds: this.transactionTags$.getValue().map(x => x.id)
    };
    this.save$.emit(transaction);
  }

  selectTag(tag: Tag) {
    let selectedTags = this.transactionTags$.getValue();
    if (selectedTags.includes(tag)) {
      selectedTags = selectedTags.filter(x => x.id != tag.id)
    } else {
      selectedTags.push(tag);
    }
      this.transactionTags$.next(selectedTags);
  }
}
