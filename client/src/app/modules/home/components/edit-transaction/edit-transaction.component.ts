import { Component, EventEmitter, Input, OnDestroy, OnInit, Output, ViewChildren } from '@angular/core';
import { BehaviorSubject, combineLatestWith, forkJoin, mergeMap, Observable, Subject, take, takeUntil } from "rxjs";
import { Transaction, TransactionType, UpdateTransaction } from "../../models/transaction";
import { FormControl, FormGroup, Validators } from "@angular/forms";
import { Account, Currency } from "../../models/account";
import { Tag, TagType } from "../../models/tag";
import { CurrencyCodeDirective } from "../../../../shared/directives/currency-code.directive";

@Component({
  selector: 'app-edit-transaction',
  templateUrl: './edit-transaction.component.html',
  styleUrls: ['./edit-transaction.component.scss']
})
export class EditTransactionComponent implements OnInit, OnDestroy {
  private id!: number;
  @ViewChildren(CurrencyCodeDirective) directives!: CurrencyCodeDirective[];
  @Input() public transaction$!: Observable<Transaction>;
  @Input() public accounts$!: Observable<Account[]>;
  @Input() public tags$!: Observable<Tag[]>
  @Output() public save$: EventEmitter<UpdateTransaction> = new EventEmitter<UpdateTransaction>();
  @Output() public delete$: EventEmitter<never> = new EventEmitter<never>();
  @Output() public goBack$: EventEmitter<never> = new EventEmitter<never>()
  private accounts: Account[] = [];
  private notifier$: Subject<never> = new Subject<never>();
  public transactionTags$: BehaviorSubject<Tag[]> = new BehaviorSubject<Tag[]>([]);
  public form: FormGroup = new FormGroup({
    account: new FormControl('', [Validators.required]),
    name: new FormControl('', [Validators.required, Validators.maxLength(40)]),
    amount: new FormControl('', [Validators.required]),
    date: new FormControl(new Date().toJSON().split('T')[0], [Validators.required]),
  });

  public get AccountFromCurrency(): Currency {
    return this.accounts.find(x => x.id === +this.form.controls['account'].value)?.currency ?? Currency.notDefined;
  }

  public ngOnInit(): void {
    this.accounts$.pipe(takeUntil(this.notifier$))
      .subscribe(x => {
        this.accounts = x;
        this.form.controls['account'].setValue(x[0].id);
      });

    this.transaction$
      .pipe(take(1), combineLatestWith(this.tags$, this.accounts$))
      .subscribe(x => {
        this.form.controls['account'].setValue(x[0].accountId);
        this.form.controls['name'].setValue(x[0].name);
        this.form.controls['amount'].setValue(x[0].amount);
        this.form.controls['date'].setValue((new Date(x[0].date).toJSON().split('T')[0]));
        this.transactionTags$.next(this.flattenTags(x[1]).filter(y => x[0].tags.map(z => z.id).includes(y.id)));
        this.id = x[0].id;

        const account = x[2].find(account => account.id === x[0].accountId)!.currency;
        // though timeout value is 0, it helps to properly initialize the directive
        setTimeout(() => {
          this.directives.forEach(x => {
            x.initWithCurrency(account)
          });
        });
     });

    this.tags$.pipe(takeUntil(this.notifier$))
      .subscribe(x => {
        this.tags$ = new BehaviorSubject<Tag[]>(this.flattenTags(x));
      });

    this.form.controls['account'].valueChanges
      .pipe(takeUntil(this.notifier$))
      .subscribe(x => {
        this.directives.forEach(directive => {
          directive.currencyChanged(this.accounts.find(account => account.id === +x)!.currency);
        });
      })
  }

  private flattenTags(tags: Tag[]): Tag[] {
    const flattenedTags: Tag[] = [];
    const getChildren = (tags: Tag[]) => {
      tags.forEach(tag => {
        flattenedTags.push(tag);
        if (tag.children.length > 0) {
          getChildren(tag.children);
        }
      });
    };
    getChildren(tags);
    return flattenedTags;
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
    const formattedAmount = this.form.controls['amount'].value.toString().replace(',', '.').trim();
    const transaction: UpdateTransaction = {
      id: this.id,
      name: this.form.controls['name'].value,
      type: this.form.controls['amount'].value > 0 ? TransactionType.income : TransactionType.expense,
      amount: +formattedAmount,
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

  protected readonly TagType = TagType;
}
