import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FormControlValidationErrorComponent } from './form-control-validation-error.component';

describe('FormControlValidationErrorComponent', () => {
  let component: FormControlValidationErrorComponent;
  let fixture: ComponentFixture<FormControlValidationErrorComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ FormControlValidationErrorComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(FormControlValidationErrorComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
