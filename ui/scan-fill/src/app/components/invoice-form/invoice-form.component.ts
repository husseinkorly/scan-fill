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

  submissionForm: FormGroup = this.fb.group({});

  constructor(private fb: FormBuilder) { }

  ngOnChanges(changes: SimpleChanges): void{
    if (!changes['invoiceFormData'].firstChange) {
      this.createForm(changes['invoiceFormData'].currentValue);
      //this.invoiceForm.patchValue(changes.invoiceFormData.currentValue);
    }
  }

  createForm(controles: InvoiceFormData) {
    for (const control of controles.controls) {
      this.submissionForm.addControl(control.name, this.fb.control(control.value));
    }
  }

  onSubmit() {
    console.log(this.submissionForm.value);
  }
}
