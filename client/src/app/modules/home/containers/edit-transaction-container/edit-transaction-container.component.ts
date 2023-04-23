import { Component, OnInit } from '@angular/core';
import { ReplaySubject, take } from "rxjs";
import { Transaction, UpdateTransaction } from "../../models/transaction";
import { ActivatedRoute, Router } from "@angular/router";
import { TransactionsService } from "../../services/transactions.service";
import { TagService } from "../../services/tag.service";
import { AccountsService } from "../../services/accounts.service";
import { Tag } from "../../models/tag";
import { Account } from "../../models/account";

@Component({
  selector: 'app-edit-transaction-container',
  templateUrl: './edit-transaction-container.component.html',
  styleUrls: ['./edit-transaction-container.component.scss']
})
export class EditTransactionContainerComponent implements OnInit {
  private id!: number;
  public transaction$: ReplaySubject<Transaction> = new ReplaySubject<Transaction>();
  public accounts$: ReplaySubject<Account[]> = new ReplaySubject<Account[]>();
  public tags$: ReplaySubject<Tag[]> = new ReplaySubject<Tag[]>();

  public constructor(private service: TransactionsService, private tagsService: TagService,
                     private accountsService: AccountsService, private route: ActivatedRoute, private router: Router) {
  }

  public ngOnInit(): void {
    this.getIdFromRoute();
    this.accountsService.getAllAccounts()
      .pipe(take(1))
      .subscribe({
        next: response => this.accounts$.next(response.body)
      });
    this.tagsService.getAllTags()
      .pipe(take(1))
      .subscribe({
        next: response => this.tags$.next(response.body)
      });
  }

  private getIdFromRoute() {
    this.route.params
      .pipe(take(1))
      .subscribe(result => {
        const id = result["id"];
        this.id = id;
        if (id > 0) {
          this.getTransaction(id);
        }
      });
  }
  private getTransaction(id: number): void {
    this.service.getTransaction(id)
      .pipe(take(1))
      .subscribe({
        next: response => {
          this.transaction$.next(response.body);
        }
      });
  }

  public save(transaction: UpdateTransaction) {
    if (transaction.id > 0) {
      this.service.updateTransaction(transaction)
        .pipe(take(1))
        .subscribe({
          next: () => this.goBack()
        });
    } else {
      this.service.createTransaction(transaction).pipe(take(1))
        .subscribe({
          next: () => this.goBack()
        });
    }
  }

  public delete(): void {
    this.service.deleteTransaction(this.id)
      .pipe(take(1))
      .subscribe({
        next: () => this.goBack()
      });
  }

  public goBack(): void {
    this.router.navigateByUrl('');
  }
}
