import { Component, EventEmitter, Input, Output } from '@angular/core';
import { AccountWithBalance } from "../../models/account";
import { Observable } from "rxjs";

@Component({
  selector: 'app-accounts-list',
  templateUrl: './accounts-list.component.html',
  styleUrls: ['./accounts-list.component.scss']
})
export class AccountsListComponent {
  @Input() public accounts$!: Observable<AccountWithBalance[]>;
  @Output() public showDetails$: EventEmitter<number> = new EventEmitter<number>();
  @Output() public addAccount$: EventEmitter<never> = new EventEmitter<never>();
}
