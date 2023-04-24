import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Observable } from "rxjs";
import { Account } from "../../models/account";

@Component({
  selector: 'app-accounts-list',
  templateUrl: './accounts-list.component.html',
  styleUrls: ['./accounts-list.component.scss']
})
export class AccountsListComponent {
  @Input() public accounts$!: Observable<Account[]>;
  @Output() public showDetails$: EventEmitter<number> = new EventEmitter<number>();
  @Output() public addAccount$: EventEmitter<never> = new EventEmitter<never>();
  @Output() public transfer$: EventEmitter<never> = new EventEmitter<never>();
}
