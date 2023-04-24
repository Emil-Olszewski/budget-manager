import { ReplaySubject, take, tap } from 'rxjs';
import { Component } from '@angular/core';
import { TransferTransaction, TransferTransactionModel } from '../../models/transfer-transaction';
import { Account } from '../../models/account';
import { TransactionsService } from '../../services/transactions.service';
import { AccountsService } from '../../services/accounts.service';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-edit-transfer-container',
  templateUrl: './edit-transfer-container.component.html',
  styleUrls: ['./edit-transfer-container.component.scss']
})
export class EditTransferContainerComponent {
  private id!: number;
  private transactionId!: number;
  public transfer$: ReplaySubject<TransferTransaction> = new ReplaySubject<TransferTransaction>();
  public accounts$: ReplaySubject<Account[]> = new ReplaySubject<Account[]>();

  public constructor(private service: TransactionsService, private accountsService: AccountsService,
      private route: ActivatedRoute, private router: Router) {
  }

  public ngOnInit(): void {
    this.getIdFromRoute();
    this.accountsService.getAllAccounts()
      .pipe(take(1))
      .subscribe({
        next: response => this.accounts$.next(response.body)
      });
  }

  private getIdFromRoute() {
    this.route.params
      .pipe(take(1))
      .subscribe(result => {
        const id = result["id"];
        this.id = id;
        if (id > 0) {
          this.getTransfer(id);
        }
      });
  }

  private getTransfer(id: number): void {
    this.service.getTransferTransaction(id)
      .pipe(take(1), tap(x => this.transactionId = x.body.outputTransaction.id))
      .subscribe({
        next: response => {
          this.transfer$.next(response.body);
        }
      });
  }

  public save(transaction: TransferTransactionModel) {
    if (transaction.id > 0) {
      this.service.updateTransferTransaction(transaction)
        .pipe(take(1))
        .subscribe({
          next: () => this.goBack()
        });
    } else {
      this.service.createTransferTransaction(transaction).pipe(take(1))
        .subscribe({
          next: () => this.goBack()
        });
    }
  }

  public delete(): void {
    this.service.deleteTransaction(this.transactionId)
      .pipe(take(1))
      .subscribe({
        next: () => this.goBack()
      });
  }

  public goBack(): void {
    this.router.navigateByUrl('');
  }
}
