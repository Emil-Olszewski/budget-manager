import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ErrorMessageComponent } from './components/error-message/error-message.component';
import { FormControlValidationErrorComponent } from './components/form-control-validation-error/form-control-validation-error.component';
import { FormControlValidationDirective } from './directives/form-control-validation.directive';
import { CurrencyCodeDirective } from './directives/currency-code.directive';
import { CurrencyCodePipe } from "./pipes/currency-code.pipe";


@NgModule({
  declarations: [
    ErrorMessageComponent,
    FormControlValidationErrorComponent,
    FormControlValidationDirective,
    CurrencyCodeDirective,
    CurrencyCodePipe
    ],
    exports: [
        ErrorMessageComponent,
        FormControlValidationErrorComponent,
        FormControlValidationDirective,
        CurrencyCodeDirective,
        CurrencyCodePipe
    ],
  imports: [
    CommonModule
  ]
})
export class SharedModule { }
