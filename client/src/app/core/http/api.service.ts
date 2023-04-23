import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { environment } from "../../environments/environment";
import { ToastService } from "../toaster/toast.service";

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  public constructor(private http: HttpClient, private toastService: ToastService) {
  }

  public get<T>(path: string, params: any = new HttpParams(), responseType = 'json'): Observable<T> {
    const options = {
      observe: 'response' as const,
      params,
      responseType
    };
    return this.http.get<T>(`${environment.apiUrl}/${path}`, options as any)
      .pipe(catchError(errors => this.formatErrors(errors)));
  }

  public put(path: string, body: object = {}): Observable<any> {
    const options = {
      headers: new HttpHeaders({ 'Content-Type': 'application/json' })
    };
    return this.http.put(`${environment.apiUrl}/${path}`, JSON.stringify(body), options)
      .pipe(catchError(errors => this.formatErrors(errors)));
  }

  public post(path: string, body: object = {}): Observable<any> {
    const options = {
      headers: new HttpHeaders({ 'Content-Type': 'application/json' })
    };
    return this.http.post(`${environment.apiUrl}/${path}`,JSON.stringify(body),options)
      .pipe(catchError(errors => this.formatErrors(errors)));
  }

  public delete(path: string): Observable<any> {
    return this.http.delete(`${environment.apiUrl}/${path}`)
      .pipe(catchError(errors => this.formatErrors(errors)));
  }

  private formatErrors(error: any): Observable<any> {
    this.toastService.showErrorToast('Error', error.error.message);
    return throwError(error.error);
  }
}
