import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { DashboardPageComponent } from "./pages/dashboard-page/dashboard-page.component";
import { EditAccountPageComponent } from "./pages/edit-account-page/edit-account-page.component";
import { TagsPageComponent } from "./pages/tags-page/tags-page.component";
import { EditTagPageComponent } from "./pages/edit-tag-page/edit-tag-page.component";
import { EditTransactionPageComponent } from "./pages/edit-transaction-page/edit-transaction-page.component";

const routes: Routes = [
  { path: 'transactions/edit/:id', component: EditTransactionPageComponent },
  { path: 'tags/edit/:id', component: EditTagPageComponent },
  { path: 'tags', component: TagsPageComponent },
  { path: 'accounts/edit/:id', component: EditAccountPageComponent },
  { path: '', component: DashboardPageComponent }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class HomeRoutingModule { }
