import { Component, OnInit } from '@angular/core';
import { ReplaySubject, take } from 'rxjs';
import { AccountsService } from "../../services/accounts.service";
import { Account } from "../../models/account";
import { Router } from "@angular/router";
import { TransactionsService } from "../../services/transactions.service";
import { Transaction } from "../../models/transaction";


@Component({
  selector: 'app-dashboard-container',
  templateUrl: './dashboard-container.component.html',
  styleUrls: ['./dashboard-container.component.scss']
})
export class DashboardContainerComponent implements OnInit {
  public accounts$: ReplaySubject<Account[]> = new ReplaySubject<Account[]>();
  public transactions$: ReplaySubject<Transaction[]> = new ReplaySubject<Transaction[]>();
  constructor(private accountsService: AccountsService, private transactionsService: TransactionsService, private router: Router) { }
  public ngOnInit(): void {
    this.accountsService.getAllAccounts()
      .pipe(take(1))
      .subscribe({
        next: response => this.accounts$.next(response.body),
      });
    this.transactionsService.getAllTransactions()
      .pipe(take(1))
      .subscribe({
        next: response => this.transactions$.next(response.body)
      });
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

  public transfer(): void {
    this.router.navigate(['transfers', 'edit', 0]);
    }
}
