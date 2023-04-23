import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ErrorMessageComponent } from './components/error-message/error-message.component';
import { FormControlValidationErrorComponent } from './components/form-control-validation-error/form-control-validation-error.component';
import { FormControlValidationDirective } from './directives/form-control-validation.directive';
import { CurrencyNamePipe } from './pipes/currency-name.pipe';



@NgModule({
  declarations: [
    ErrorMessageComponent,
    FormControlValidationErrorComponent,
    FormControlValidationDirective,
    CurrencyNamePipe,
    ],
  exports: [
    ErrorMessageComponent,
    FormControlValidationErrorComponent,
    FormControlValidationDirective,
    CurrencyNamePipe
  ],
  imports: [
    CommonModule
  ]
})
export class SharedModule { }
