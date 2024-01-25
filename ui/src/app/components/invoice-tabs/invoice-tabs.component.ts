import { Component, Input } from '@angular/core';
import { InvoiceFormData } from '../invoice-form/invoice-form.component';

@Component({
  selector: 'app-invoice-tabs',
  templateUrl: './invoice-tabs.component.html',
  styleUrls: ['./invoice-tabs.component.css']
})

export class InvoiceTabsComponent {
  @Input()
  invoiceFormData!: InvoiceFormData;
  invoices: any = [];

  ngOnChanges(): void {
    this.invoices = this.invoiceFormData;
    this.invoices = this.invoices?.filter((invoice: any) => invoice.LineItems.length > 0);
    this.invoiceFormData = this.invoices;
  }
}
