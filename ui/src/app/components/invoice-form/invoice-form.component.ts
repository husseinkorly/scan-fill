import { Component, OnChanges, Input, SimpleChanges } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';

interface FromControl {
  name: string;
  label: string;
  value: string;
  type: string;
}

export interface InvoiceFormData {
  controls: FromControl[];
}

@Component({
  selector: 'app-invoice-form',
  templateUrl: './invoice-form.component.html',
  styleUrls: ['./invoice-form.component.css']
})
export class InvoiceFormComponent implements OnChanges {
  @Input()
  invoiceFormData!: InvoiceFormData;
  displayInvoiceForm = false;

  submissionForm: FormGroup = this.fb.group({
    InvoiceId: [''],
    InvoiceDate: [''],
    PurchaseOrder: [''],
  });

  constructor(private fb: FormBuilder) { }

  ngOnChanges(changes: SimpleChanges): void{
    if (!changes['invoiceFormData'].firstChange) {
      this.submissionForm.patchValue(this.getcontrolValues(changes['invoiceFormData'].currentValue['Header']));
      this.displayInvoiceForm = true;
    }
  }

  getcontrolValues(header: any) {
    const values: any = {};
    for (const key in header) {
      if (Object.prototype.hasOwnProperty.call(header, key)) {
        if(key == 'InvoiceDate') {
          const d = new Date(header[key]);
          let month = '' + (d.getMonth() + 1);
          let day = '' + d.getDate();
          const year = d.getFullYear();
          if (month.length < 2) month = '0' + month;
          if (day.length < 2) day = '0' + day;
          
          values[key] = [year, month, day].join('-');
        }
        else {
          values[key] = header[key];
        }
      }
    }
    
    return values;
  }

  onSubmit() {
    console.log(this.submissionForm.value);
  }
}
