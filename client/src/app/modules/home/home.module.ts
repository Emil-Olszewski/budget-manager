import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from "../../shared/shared.module";
import { HomeRoutingModule } from './home-routing.module';
import { DashboardPageComponent } from './pages/dashboard-page/dashboard-page.component';
import { DashboardContainerComponent } from './containers/dashboard-container/dashboard-container.component';
import { AccountsListComponent } from './components/accounts-list/accounts-list.component';
import { EditAccountPageComponent } from './pages/edit-account-page/edit-account-page.component';
import { EditAccountContainerComponent } from './containers/edit-account-container/edit-account-container.component';
import { EditAccountComponent } from './components/edit-account/edit-account.component';
import { ReactiveFormsModule } from "@angular/forms";
import { TagsPageComponent } from './pages/tags-page/tags-page.component';
import { TagsContainerComponent } from './containers/tags-container/tags-container.component';
import { TagsListComponent } from './components/tags-list/tags-list.component';
import { EditTagPageComponent } from './pages/edit-tag-page/edit-tag-page.component';
import { EditTagContainerComponent } from './containers/edit-tag-container/edit-tag-container.component';
import { EditTagComponent } from './components/edit-tag/edit-tag.component';
import { EditTransactionPageComponent } from './pages/edit-transaction-page/edit-transaction-page.component';
import { EditTransactionContainerComponent } from './containers/edit-transaction-container/edit-transaction-container.component';
import { EditTransactionComponent } from './components/edit-transaction/edit-transaction.component';
import { TransactionsListComponent } from './components/transactions-list/transactions-list.component';
import { EditTransferPageComponent } from './pages/edit-transfer-page/edit-transfer-page.component';
import { EditTransferContainerComponent } from './containers/edit-transfer-container/edit-transfer-container.component';
import { EditTransferComponent } from './components/edit-transfer/edit-transfer.component';
import { TransactionsStatsComponent } from './components/transactions-stats/transactions-stats.component';



@NgModule({
  declarations: [
    DashboardPageComponent,
    DashboardContainerComponent,
    AccountsListComponent,
    EditAccountPageComponent,
    EditAccountContainerComponent,
    EditAccountComponent,
    TagsPageComponent,
    TagsContainerComponent,
    TagsListComponent,
    EditTagPageComponent,
    EditTagContainerComponent,
    EditTagComponent,
    EditTransactionPageComponent,
    EditTransactionContainerComponent,
    EditTransactionComponent,
    TransactionsListComponent,
    EditTransferPageComponent,
    EditTransferContainerComponent,
    EditTransferComponent,
    TransactionsStatsComponent,
  ],
  imports: [
    CommonModule,
    SharedModule,
    HomeRoutingModule,
    ReactiveFormsModule
  ]
})
export class HomeModule { }
