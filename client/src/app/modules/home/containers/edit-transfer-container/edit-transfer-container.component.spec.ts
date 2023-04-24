import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EditTransferContainerComponent } from './edit-transfer-container.component';

describe('EditTransferContainerComponent', () => {
  let component: EditTransferContainerComponent;
  let fixture: ComponentFixture<EditTransferContainerComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ EditTransferContainerComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(EditTransferContainerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
