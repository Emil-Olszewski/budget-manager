import { UpdateTransferTransaction } from './../models/transfer-transaction';
import { Injectable } from '@angular/core';
import { ApiService } from "../../../core/http/api.service";
import { Observable } from "rxjs";
import { ApiResponse } from "../../../shared/models/api-response";
import { CreateTransaction, Transaction, TransferTransactionShort, UpdateTransaction } from "../models/transaction";
import { CreateTransferTransaction, TransferTransaction } from '../models/transfer-transaction';

@Injectable({
  providedIn: 'root'
})
export class TransactionsService {
  public constructor(private service: ApiService) { }

  public getAllTransactions(from?: Date, to?: Date): Observable<ApiResponse<Transaction[]>> {
    let path = 'transactions';
    if (from && to) {
      path += `?from=${from.toISOString()}&to=${to.toISOString()}`;
    } else if (from) {
      path += `?from=${from.toISOString()}`;
    } else if (to) {
      path += `?to=${to.toISOString()}`;
    }
    return this.service.get(path);
  }

  public getAllTransferTransactions(from?: Date, to?: Date): Observable<ApiResponse<TransferTransactionShort[]>>{
    let path = 'transactions/transferTransactions';
    if (from && to) {
      path += `?from=${from.toISOString()}&to=${to.toISOString()}`;
    } else if (from) {
      path += `?from=${from.toISOString()}`;
    } else if (to) {
      path += `?to=${to.toISOString()}`;
    }
    return this.service.get(path);
  }

  public getTransaction(id: number): Observable<ApiResponse<Transaction>> {
    return this.service.get("transactions/" + id);
  }

  public getTransferTransaction(id: number): Observable<ApiResponse<TransferTransaction>> {
    return this.service.get('transactions/transferTransactions/' + id)
  }

  public createTransaction(transaction: CreateTransaction): Observable<undefined> {
    return this.service.post("transactions", transaction);
  }

  public createTransferTransaction(transaction: CreateTransferTransaction): Observable<undefined> {
    return this.service.post("transactions/transferTransactions", transaction);
  }

  public updateTransaction(transaction: UpdateTransaction): Observable<undefined> {
    return this.service.put("transactions/" + transaction.id, transaction);
  }

  public updateTransferTransaction(transaction: UpdateTransferTransaction): Observable<undefined> {
    return this.service.put("transactions/transferTransactions/" + transaction.id, transaction);
  }

  public deleteTransaction(id: number): Observable<undefined> {
    return this.service.delete("transactions/" + id);
  }
}
