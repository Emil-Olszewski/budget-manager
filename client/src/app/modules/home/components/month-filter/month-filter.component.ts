import { Component, OnDestroy, OnInit } from '@angular/core';
import { Month } from "../../models/month";
import { ReplaySubject, Subject, take, takeUntil } from "rxjs";
import { FormControl, FormGroup } from "@angular/forms";
import { ActivatedRoute, Router } from "@angular/router";

@Component({
  selector: 'app-month-filter',
  templateUrl: './month-filter.component.html',
  styleUrls: ['./month-filter.component.scss']
})
export class MonthFilterComponent implements OnInit, OnDestroy {
  private notifier$: Subject<never> = new Subject<never>();
  private lock: boolean = false;
  public form: FormGroup = new FormGroup({
    year: new FormControl(),
    month : new FormControl(),
  });
  constructor(private route: ActivatedRoute, private router: Router) {
  }

  protected readonly Month = Month;

  public ngOnInit(): void {
    this.route.queryParams
      .pipe(takeUntil(this.notifier$))
      .subscribe(params => {
        if (this.lock) {
          return;
        }
        this.lock = true;
        const dateFrom = params['dateFrom'];
        if (dateFrom) {
          const year = new Date(dateFrom).getFullYear();
          const month = new Date(dateFrom).getMonth() + 1;
          this.form.setValue({year, month});
        }
        this.lock = false;
      });

    this.form.valueChanges
      .pipe(takeUntil(this.notifier$))
      .subscribe(value => {
        if (this.lock) {
          return;
        }
        this.lock = true;

        const newDateFrom = new Date(value.year, value.month - 1, 2);

        const newDateTo = new Date(newDateFrom);
        newDateTo.setMonth(newDateTo.getMonth() + 1);
        newDateTo.setDate(newDateTo.getDate() - 1);

        const newDateFromFormatted = newDateFrom.toISOString().split('T')[0];
        const newDateToFormatted = newDateTo.toISOString().split('T')[0];

        this.router.navigate([], {queryParams: {dateFrom: newDateFromFormatted, dateTo: newDateToFormatted}});
        this.lock = false;
      });
  }

  public ngOnDestroy(): void {
    this.notifier$.next(null as never);
    this.notifier$.complete();
  }
}
