import { Component, OnChanges, Input, SimpleChanges } from '@angular/core';
import { InvoiceFormData } from '../invoice-form/invoice-form.component';
import { FormBuilder } from '@angular/forms';

@Component({
  selector: 'app-line-form',
  templateUrl: './line-form.component.html',
  styleUrls: ['./line-form.component.css']
})
export class LineFormComponent implements OnChanges{
  @Input()
  invoiceFormData!: InvoiceFormData;
  lineItems: any = [];

  // TODO: adding dynamic form controls
  submissionForm: any = this.fb.group({
    Description: [''],
    Quantity: [''],
    UnitPrice: ['']
  });

  constructor(private fb: FormBuilder) { }

  ngOnChanges(changes: SimpleChanges): void {
    if (!changes['invoiceFormData'].firstChange) {
      this.lineItems = changes['invoiceFormData'].currentValue['LineItems'];
      console.log(this.lineItems);
    }
  }
}
