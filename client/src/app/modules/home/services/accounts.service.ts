import { Injectable } from "@angular/core";
import { ApiService } from "../../../core/http/api.service";
import { Observable } from "rxjs";
import { ApiResponse } from "../../../shared/models/api-response";
import { Account, AccountWithInitialBalance, CreateAccount, UpdateAccount } from "../models/account";


@Injectable({
  providedIn: 'root'
})
export class AccountsService {
  public constructor(private service: ApiService) { }

  public getAllAccounts(): Observable<ApiResponse<Account[]>> {
    return this.service.get("accounts");
  }

  public getAccount(id: number): Observable<ApiResponse<AccountWithInitialBalance>> {
    return this.service.get("accounts/" + id);
  }

  public createAccount(account: CreateAccount): Observable<undefined> {
    return this.service.post("accounts", account);
  }

  public updateAccount(account: UpdateAccount): Observable<undefined> {
    return this.service.put("accounts/" + account.id, account);
  }

  public deleteAccount(id: number): Observable<undefined> {
    return this.service.delete("accounts/" + id);
  }
}
