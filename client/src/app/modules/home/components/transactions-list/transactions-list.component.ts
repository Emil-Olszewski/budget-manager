import { Component, EventEmitter, Input, OnDestroy, OnInit, Output } from '@angular/core';
import { Transaction, TransactionType, TransferTransactionShort } from "../../models/transaction";
import { Observable, Subject, take, takeUntil } from "rxjs";
import { Account, Currency } from "../../models/account";
import { formatDate } from "@angular/common";

@Component({
  selector: 'app-transactions-list',
  templateUrl: './transactions-list.component.html',
  styleUrls: ['./transactions-list.component.scss']
})
export class TransactionsListComponent implements OnInit, OnDestroy {
  public dates: string[] = [];
  private notifier$: Subject<never> = new Subject<never>();
  @Input() public transactions$!: Observable<Transaction[]>;
  @Input() public transferTransactions$!: Observable<TransferTransactionShort[]>;
  @Input() public accounts$!: Observable<Account[]>;
  @Output() public addTransaction$: EventEmitter<never> = new EventEmitter<never>();
  @Output() public showDetails$: EventEmitter<number> = new EventEmitter<number>();
  @Output() public showTransferDetails$: EventEmitter<number> = new EventEmitter<number>();

  public ngOnInit(): void {
    this.transactions$.pipe(takeUntil(this.notifier$))
      .subscribe(transactions => {
        const dates = transactions.map(x => formatDate(x.date, 'yyyy-MM-dd', 'en'));
        this.dates = dates.filter((value, index, self) => self.indexOf(value) === index);
      });
  }

  public ngOnDestroy(): void {
    this.notifier$.next(null as never);
    this.notifier$.complete();
  }

  public isStripped(transaction: Transaction): boolean {
    const date = formatDate(transaction.date, 'yyyy-MM-dd', 'en');
    return this.dates.indexOf(date) % 2 !== 0;
  }

  public getColorForType(type: TransactionType): string {
    switch (type) {
      case TransactionType.expense:
        return 'text-danger';
      case TransactionType.income:
        return 'text-success';
      default:
        return 'text-primary';
    }
  }

  public getAccountName(accountId: number, accounts: Account[]): string {
    return accounts.filter(x => x.id == accountId)[0].name;
  }

  public getTransactionName(transaction: Transaction, transferTransactions: TransferTransactionShort[], accounts: Account[]): string {
    if (transaction.type !== TransactionType.transfer) {
      return transaction.name;
    }

    const transfer = transferTransactions.find(x => x.inputTransactionId == transaction.id || x.outputTransactionId == transaction.id);
    if (transaction.amount < 0) {
      return `To ${this.getAccountName(transfer!.accountToId, accounts)}`
    }
    return `From ${this.getAccountName(transfer!.accountFromId, accounts)}`
  }

  public getAccountCurrency(accountId: number, accounts: Account[]): Currency {
    return accounts.filter(x => x.id == accountId)[0].currency;
  }

  public showDetails(transaction: Transaction, transferTransactions: TransferTransactionShort[]): void {
    if (transaction.type === TransactionType.transfer) {
      const transfer = transferTransactions.find(x => x.inputTransactionId == transaction.id || x.outputTransactionId == transaction.id);
      this.showTransferDetails$.emit(transfer!.id);
    } else {
      this.showDetails$.emit(transaction.id);
    }
  }
}
