import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EditTransactionPageComponent } from './edit-transaction-page.component';

describe('EditTransactionPageComponent', () => {
  let component: EditTransactionPageComponent;
  let fixture: ComponentFixture<EditTransactionPageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ EditTransactionPageComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(EditTransactionPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
