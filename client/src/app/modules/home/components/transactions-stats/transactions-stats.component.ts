import { Component, Input, OnInit } from '@angular/core';
import { combineLatestWith, Observable, ReplaySubject, take } from "rxjs";
import { Transaction, TransferTransactionShort } from "../../models/transaction";
import { Account, Currency } from "../../models/account";
import { StatsModel, StatsModelTag } from "./stats-model";

@Component({
  selector: 'app-transactions-stats',
  templateUrl: './transactions-stats.component.html',
  styleUrls: ['./transactions-stats.component.scss']
})
export class TransactionsStatsComponent implements OnInit {
  @Input() public accounts$!: Observable<Account[]>;
  @Input() public transactions$!: Observable<Transaction[]>;
  @Input() public transferTransactions$!: Observable<TransferTransactionShort[]>;
  public statsModel$: ReplaySubject<StatsModel> = new ReplaySubject<StatsModel>();

  public ngOnInit(): void {
    this.accounts$
      .pipe(take(1), combineLatestWith( this.transactions$, this.transferTransactions$))
      .subscribe(x => this.calc(x))
  }

  private calc(x: [Account[], Transaction[], TransferTransactionShort[]]) {
    const accounts: Account[] = x[0];
    const transactions: Transaction[] = x[1];

    const model: StatsModel = new StatsModel();

    transactions.forEach(transaction => {
      const account = accounts.find(x => x.id === transaction.accountId)!;
      model.balance += this.convertToPln(transaction.amount, account.currency);
      transaction.amount;
    });

    const transactionsPerTagPerAccount: Map<number, Map<number, Transaction[]>> = new Map<number, Map<number, Transaction[]>>();
    accounts.forEach(account => {
      const transactionsPerTag: Map<number, Transaction[]> = new Map<number, Transaction[]>();
      transactions.filter(x => x.accountId === account.id).forEach(transaction => {
        transaction.tags.forEach(tag => {
          console.log(tag);
          if (!transactionsPerTag.has(tag.id)) {
            transactionsPerTag.set(tag.id, []);
          }
          transactionsPerTag.get(tag.id)?.push(transaction);
        });
      });
      transactionsPerTagPerAccount.set(account.id, transactionsPerTag);
    });

    transactionsPerTagPerAccount.forEach((transactionsPerTag, accountId) => {
      const account = accounts.find(x => x.id === accountId)!;
      transactionsPerTag.forEach((transactions, tagId) => {
        let tagModel = model.tags.find(x => x.id === tagId);
        if (!tagModel) {
          tagModel = new StatsModelTag();
          tagModel.id = tagId;
            const allTags = transactions.map(x => x.tags).reduce((a, b) => a.concat(b), []);
            tagModel.name = allTags.find(x => x.id === tagId)!.name;
            model.tags.push(tagModel);
        }
        tagModel.balance += transactions.reduce((a, b) => a + this.convertToPln(b.amount, account.currency), 0);
      });
    });

    model.tags.forEach(tag => {
      tag.percentageOfTotal = tag.balance / model.balance * 100;
    });

    this.statsModel$.next(model);
  }

  private convertToPln(amountToAdd: number, currency: Currency): number {
    switch (currency) {
      case Currency.pln:
        return amountToAdd;
      case Currency.eur:
        return amountToAdd * 4.6;
      case Currency.usd:
        return amountToAdd * 4.17;
      default:
        return 0;
    }
  }
}
