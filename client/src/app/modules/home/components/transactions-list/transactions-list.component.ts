import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Transaction, TransactionType, TransferTransactionShort } from "../../models/transaction";
import { Observable } from "rxjs";
import { Account, Currency } from "../../models/account";

@Component({
  selector: 'app-transactions-list',
  templateUrl: './transactions-list.component.html',
  styleUrls: ['./transactions-list.component.scss']
})
export class TransactionsListComponent {
  @Input() public transactions$!: Observable<Transaction[]>;
  @Input() public transferTransactions$!: Observable<TransferTransactionShort[]>;
  @Input() public accounts$!: Observable<Account[]>;
  @Output() public addTransaction$: EventEmitter<never> = new EventEmitter<never>();
  @Output() public showDetails$: EventEmitter<number> = new EventEmitter<number>();
  @Output() public showTransferDetails$: EventEmitter<number> = new EventEmitter<number>();

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
    return `From: ${this.getAccountName(transfer!.accountFromId, accounts)} To: ${this.getAccountName(transfer!.accountToId, accounts)}`
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
