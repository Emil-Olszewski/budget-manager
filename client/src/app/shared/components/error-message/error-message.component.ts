import { Component, Input } from '@angular/core';
import { ApiErrorResponse } from "../../models/api-error-response";
import { Observable } from "rxjs";

@Component({
  selector: 'app-error-message',
  templateUrl: './error-message.component.html',
  styleUrls: ['./error-message.component.scss']
})
export class ErrorMessageComponent {
  @Input() public error$!: Observable<ApiErrorResponse | null>;

  public getErrorMessage(error: ApiErrorResponse): string {
    if (error.message == undefined) {
      return 'Wystąpił nieznany błąd. Upewnij się, że istnieje aktywne połączenie z serwerem.';
    }
    return error.message;
  }
}
