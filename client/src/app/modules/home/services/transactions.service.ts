import { Injectable } from '@angular/core';
import { ApiService } from "../../../core/http/api.service";
import { Observable } from "rxjs";
import { ApiResponse } from "../../../shared/models/api-response";
import { CreateTransaction, Transaction, UpdateTransaction } from "../models/transaction";

@Injectable({
  providedIn: 'root'
})
export class TransactionsService {
  public constructor(private service: ApiService) { }

  public getAllTransactions(): Observable<ApiResponse<Transaction[]>> {
    return this.service.get("transactions");
  }

  public getTransaction(id: number): Observable<ApiResponse<Transaction>> {
    return this.service.get("transactions/" + id);
  }

  public createTransaction(transaction: CreateTransaction): Observable<undefined> {
    return this.service.post("transactions", transaction);
  }

  public updateTransaction(transaction: UpdateTransaction): Observable<undefined> {
    return this.service.put("transactions/" + transaction.id, transaction);
  }

  public deleteTransaction(id: number): Observable<undefined> {
    return this.service.delete("transactions/" + id);
  }
}
