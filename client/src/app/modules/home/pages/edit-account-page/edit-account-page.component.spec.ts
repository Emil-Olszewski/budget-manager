import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EditAccountPageComponent } from './edit-account-page.component';

describe('EditAccountPageComponent', () => {
  let component: EditAccountPageComponent;
  let fixture: ComponentFixture<EditAccountPageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ EditAccountPageComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(EditAccountPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
