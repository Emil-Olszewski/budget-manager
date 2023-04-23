import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EditTransactionContainerComponent } from './edit-transaction-container.component';

describe('EditTransactionContainerComponent', () => {
  let component: EditTransactionContainerComponent;
  let fixture: ComponentFixture<EditTransactionContainerComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ EditTransactionContainerComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(EditTransactionContainerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
