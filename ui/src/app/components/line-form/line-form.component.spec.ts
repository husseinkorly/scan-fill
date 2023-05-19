import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LineFormComponent } from './line-form.component';

describe('LineFormComponent', () => {
  let component: LineFormComponent;
  let fixture: ComponentFixture<LineFormComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [LineFormComponent]
    });
    fixture = TestBed.createComponent(LineFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
