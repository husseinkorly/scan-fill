import { Component } from '@angular/core';
import { InvoiceFormData } from './components/invoice-form/invoice-form.component';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'Scan Fill';
  formData!: InvoiceFormData;

  setInvoiceFormData(event: InvoiceFormData) {
    this.formData = event;
  }
}
