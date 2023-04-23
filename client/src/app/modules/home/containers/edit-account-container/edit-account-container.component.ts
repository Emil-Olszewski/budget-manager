import { Component, OnInit } from '@angular/core';
import { AccountsService } from "../../services/accounts.service";
import { ActivatedRoute, Router } from "@angular/router";
import { ReplaySubject, take } from "rxjs";
import { AccountWithInitialBalance, UpdateAccount } from "../../models/account";

@Component({
  selector: 'app-edit-account-container',
  templateUrl: './edit-account-container.component.html',
  styleUrls: ['./edit-account-container.component.scss']
})
export class EditAccountContainerComponent implements OnInit {
  private id!: number;
  public account$: ReplaySubject<AccountWithInitialBalance> = new ReplaySubject<AccountWithInitialBalance>();
  public constructor(private service: AccountsService, private route: ActivatedRoute, private router: Router) {
  }

  public ngOnInit() {
    this.getIdFromRoute();
  }

  private getIdFromRoute() {
    this.route.params
      .pipe(take(1))
      .subscribe(result => {
        const id = result["id"];
        this.id = id;
        if (id > 0) {
          this.getAccount(id);
        }
      });
  }

  private getAccount(id: number) {
    this.service.getAccount(id)
      .pipe(take(1))
      .subscribe({
        next: response => {
          this.account$.next(response.body);
        }
      });
  }

  public save(account: UpdateAccount) {
    if (account.id > 0) {
      this.service.updateAccount(account)
        .pipe(take(1))
        .subscribe({
          next: () => this.router.navigateByUrl('')
        });
    } else {
      this.service.createAccount(account).pipe(take(1))
        .subscribe({
          next: () => this.router.navigateByUrl('')
        });
    }
  }

  public delete(): void {
    this.service.deleteAccount(this.id)
      .pipe(take(1))
      .subscribe({
        next: () => this.router.navigateByUrl('')
      });
  }

  public goBack(): void {
    this.router.navigateByUrl('');
  }
}
