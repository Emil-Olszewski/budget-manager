import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EditAccountContainerComponent } from './edit-account-container.component';

describe('EditAccountContainerComponent', () => {
  let component: EditAccountContainerComponent;
  let fixture: ComponentFixture<EditAccountContainerComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ EditAccountContainerComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(EditAccountContainerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
