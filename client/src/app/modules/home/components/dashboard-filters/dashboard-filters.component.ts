import { Component, EventEmitter, Input, OnDestroy, OnInit, Output } from '@angular/core';
import { FormArray, FormBuilder, FormControl, FormGroup } from "@angular/forms";
import { Observable, Subject, take, takeUntil } from "rxjs";
import { Account } from "../../models/account";
import { ActivatedRoute } from "@angular/router";
import { FilteringSpec } from "../../containers/dashboard-container/filtering-spec";
import { LocalStorageService } from "../../../../core/local-storage/local-storage.service";

@Component({
  selector: 'app-dashboard-filters',
  templateUrl: './dashboard-filters.component.html',
  styleUrls: ['./dashboard-filters.component.scss']
})
export class DashboardFiltersComponent implements OnInit, OnDestroy {
  private accounts!: Account[] ;
  private notifier$: Subject<never> = new Subject<never>();
  @Input() public accounts$!: Observable<Account[]>;
  @Output() public applyFilters: EventEmitter<FilteringSpec> = new EventEmitter<FilteringSpec>();
  public form: FormGroup = new FormGroup({});

  public get controls(): FormArray<FormControl> {
    return this.form.get('accounts') as FormArray<FormControl>;
  }

  constructor(private builder: FormBuilder, private route: ActivatedRoute, private storage: LocalStorageService) {
  }

  public ngOnInit(): void {
    this.accounts$.pipe(take(1))
      .subscribe((accounts) => {
        this.accounts = accounts;
        this.form = this.builder.group({
          accounts: this.buildControls(accounts)
        });
        const spec = this.storage.getItem('filteringSpec') as FilteringSpec;
        if (spec) {
          this.form.patchValue({
            accounts: accounts.map(account => {
              return spec.accountIds.includes(account.id);
            })
          });
        }
        this.form.valueChanges.pipe(takeUntil(this.notifier$))
          .subscribe(x => {
            this.apply();
          });
      });
  }

  public ngOnDestroy(): void {
    this.notifier$.next(null as never);
    this.notifier$.complete();
  }

  private buildControls(accounts: Account[]) {
    const controls = accounts.map(account => {
      return this.builder.control(true);
    });
    return this.builder.array(controls);
  }

  public apply() {
    const selectedAccountIds: number[] = [];
    this.controls.controls
      .forEach((control, index) => {
        if (control.value) {
          selectedAccountIds.push(this.accounts[index].id);
        }
      });

    const spec = new FilteringSpec();
    spec.accountIds = selectedAccountIds;
    this.applyFilters.emit(spec);
  }
}
