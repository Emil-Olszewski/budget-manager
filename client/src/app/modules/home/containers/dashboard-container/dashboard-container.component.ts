import { Component, OnDestroy, OnInit } from '@angular/core';
import { BehaviorSubject, ReplaySubject, Subject, take, takeUntil } from 'rxjs';
import { AccountsService } from "../../services/accounts.service";
import { Account } from "../../models/account";
import { ActivatedRoute, Router } from "@angular/router";
import { TransactionsService } from "../../services/transactions.service";
import { Transaction, TransferTransactionShort } from "../../models/transaction";
import { FilteringSpec } from "./filtering-spec";
import { LocalStorageService } from "../../../../core/local-storage/local-storage.service";


@Component({
  selector: 'app-dashboard-container',
  templateUrl: './dashboard-container.component.html',
  styleUrls: ['./dashboard-container.component.scss']
})
export class DashboardContainerComponent implements OnInit, OnDestroy {
  private notifier$: Subject<never> = new Subject<never>();
  public accounts$: ReplaySubject<Account[]> = new ReplaySubject<Account[]>();
  private transactions$: BehaviorSubject<Transaction[]> = new BehaviorSubject<Transaction[]>([]);
  public filteredTransactions$: ReplaySubject<Transaction[]> = new ReplaySubject<Transaction[]>()
  public transferTransactions$: ReplaySubject<TransferTransactionShort[]> = new ReplaySubject<TransferTransactionShort[]>();
  constructor(private accountsService: AccountsService, private transactionsService: TransactionsService,
              private router: Router, private route: ActivatedRoute, private storage: LocalStorageService) { }
  public ngOnInit(): void {
    this.route.queryParams
      .pipe(takeUntil(this.notifier$))
      .subscribe(params => {
        let dateFrom = params['dateFrom'];
        let dateTo = params['dateTo'];
        if (dateFrom && !dateTo) {
          const newDateTo = new Date(dateFrom);
          newDateTo.setMonth(newDateTo.getMonth() + 1);
          newDateTo.setDate(newDateTo.getDate() - 1);
          dateTo = newDateTo;
          this.router.navigate([], {queryParams: {dateFrom, dateTo: newDateTo.toISOString()}});
        }
        if (dateTo && !dateFrom) {
          const newDateFrom = new Date(dateTo);
          newDateFrom.setDate(1);
          dateFrom = newDateFrom;
          this.router.navigate([], {queryParams: {dateFrom: newDateFrom.toISOString(), dateTo}});
        }
        if (!dateFrom && !dateTo) {
          const newDateFrom = new Date();
          newDateFrom.setDate(1);
          const newDateFromFormatted = newDateFrom.toISOString().split('T')[0];
          const newDateTo = new Date(newDateFrom);
          newDateTo.setMonth(newDateTo.getMonth() + 1);
          newDateTo.setDate(newDateTo.getDate() - 1);
          dateFrom = newDateFrom;
          dateTo = newDateTo;
          const newDateToFormatted = newDateTo.toISOString().split('T')[0];
          this.router.navigate([], {queryParams: {dateFrom: newDateFromFormatted, dateTo: newDateToFormatted}});
        }
        this.loadData(dateFrom, dateTo);
      });
  }

  public ngOnDestroy(): void {
    this.notifier$.next(null as never);
    this.notifier$.complete();
  }

  private loadData(dateFrom: Date, dateTo: Date): void {
    this.accountsService.getAllAccounts(undefined, new Date(dateTo))
      .pipe(take(1))
      .subscribe({
        next: response => this.accounts$.next(response.body),
      });
    this.transactionsService.getAllTransactions(new Date(dateFrom), new Date(dateTo))
      .pipe(take(1))
      .subscribe({
        next: response=> {
          this.transactions$.next(response.body);
          const spec = this.storage.getItem('filteringSpec') as FilteringSpec;
          if (spec) {
            this.applyFilters(spec);
          } else {
            this.filteredTransactions$.next(response.body);
          }
        }
      });
    this.transactionsService.getAllTransferTransactions(new Date(dateFrom), new Date(dateTo))
      .pipe(take(1))
      .subscribe({
        next: response => this.transferTransactions$.next(response.body)
      })
  }

  public showAccountDetails(id: number): void {
    this.router.navigate(['accounts', 'edit', id]);
  }

  public addAccount(): void  {
    this.router.navigate(['accounts', 'edit', 0]);
  }

  public addTransaction(): void {
    this.router.navigate(['transactions', 'edit', 0]);
  }

  public showTransactionDetails(id: number): void {
    this.router.navigate(['transactions', 'edit', id]);
  }

  public addTransfer(): void {
    this.router.navigate(['transfers', 'edit', 0]);
  }

  public showTransferDetails(id: number): void {
    this.router.navigate(['transfers', 'edit', id]);
  }

  public applyFilters(spec: FilteringSpec) {
    var transactions = this.transactions$.getValue();
    transactions = transactions.filter(t => spec.accountIds.includes(t.accountId));
    this.filteredTransactions$.next(transactions);
    this.storage.setItem('filteringSpec', spec);
  }
}
