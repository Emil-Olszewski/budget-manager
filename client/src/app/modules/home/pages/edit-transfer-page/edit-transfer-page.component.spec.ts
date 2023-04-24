import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EditTransferPageComponent } from './edit-transfer-page.component';

describe('EditTransferPageComponent', () => {
  let component: EditTransferPageComponent;
  let fixture: ComponentFixture<EditTransferPageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ EditTransferPageComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(EditTransferPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
