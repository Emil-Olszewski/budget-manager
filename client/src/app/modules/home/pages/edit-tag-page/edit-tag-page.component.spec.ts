import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EditTagPageComponent } from './edit-tag-page.component';

describe('EditTagPageComponent', () => {
  let component: EditTagPageComponent;
  let fixture: ComponentFixture<EditTagPageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ EditTagPageComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(EditTagPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
