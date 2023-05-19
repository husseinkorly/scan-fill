import { Component, OnChanges, Input, SimpleChanges } from '@angular/core';
import { InvoiceFormData } from '../invoice-form/invoice-form.component';
import { FormBuilder } from '@angular/forms';

@Component({
  selector: 'app-summary-form',
  templateUrl: './summary-form.component.html',
  styleUrls: ['./summary-form.component.css']
})
export class SummaryFormComponent implements OnChanges {
  @Input()
  invoiceFormData!: InvoiceFormData;

  submissionForm: any = this.fb.group({
    Subtotal: [''],
    TotalTax: [''],
    InvoiceTotal: ['']
  });

  constructor(private fb: FormBuilder) { }
  
  ngOnChanges(changes: SimpleChanges): void {
    if (!changes['invoiceFormData'].firstChange) {
      this.submissionForm.patchValue(changes['invoiceFormData'].currentValue['Summary']);
    }
  }
}
