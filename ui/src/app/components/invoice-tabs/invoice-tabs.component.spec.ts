import { ComponentFixture, TestBed } from '@angular/core/testing';

import { InvoiceTabsComponent } from './invoice-tabs.component';

describe('InvoiceTabsComponent', () => {
  let component: InvoiceTabsComponent;
  let fixture: ComponentFixture<InvoiceTabsComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [InvoiceTabsComponent]
    });
    fixture = TestBed.createComponent(InvoiceTabsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
