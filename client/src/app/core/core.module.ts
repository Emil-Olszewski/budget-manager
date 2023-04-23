import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HeaderComponent } from './header/header.component';
import { HttpClientModule } from "@angular/common/http";
import { ToasterComponent } from './toaster/toaster.component';
import { ToastComponent } from './toaster/toast/toast.component';
import { RouterLink, RouterLinkActive } from "@angular/router";



@NgModule({
  declarations: [
    HeaderComponent,
    ToasterComponent,
    ToastComponent
  ],
    exports: [
      HeaderComponent,
      ToasterComponent
    ],
  imports: [
    CommonModule,
    HttpClientModule,
    RouterLink,
    RouterLinkActive
  ]
})
export class CoreModule { }
