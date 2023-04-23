import { Component, Input } from '@angular/core';
import { AbstractControl } from "@angular/forms";

@Component({
  selector: 'app-form-control-validation-error',
  templateUrl: './form-control-validation-error.component.html',
  styleUrls: ['./form-control-validation-error.component.scss'],
})
export class FormControlValidationErrorComponent {
  @Input() public control!: AbstractControl;
}
