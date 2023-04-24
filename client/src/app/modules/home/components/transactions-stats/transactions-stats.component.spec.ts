import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TransactionsStatsComponent } from './transactions-stats.component';

describe('TransactionsStatsComponent', () => {
  let component: TransactionsStatsComponent;
  let fixture: ComponentFixture<TransactionsStatsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ TransactionsStatsComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TransactionsStatsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
