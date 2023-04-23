import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EditTagContainerComponent } from './edit-tag-container.component';

describe('EditTagContainerComponent', () => {
  let component: EditTagContainerComponent;
  let fixture: ComponentFixture<EditTagContainerComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ EditTagContainerComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(EditTagContainerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
