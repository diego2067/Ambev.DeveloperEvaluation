import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms'; 
import { CommonModule } from '@angular/common'; 


import { SalesListComponent } from './components/sales-list/sales-list.component';
import { SalesCreateComponent } from './components/sales-create/sales-create.component';
import { SalesEditComponent } from './components/sales-edit/sales-edit.component';


const routes: Routes = [
  { path: '', component: SalesListComponent },
  { path: 'create', component: SalesCreateComponent },
  { path: 'edit/:id', component: SalesEditComponent },
];

@NgModule({
  declarations: [SalesListComponent, SalesCreateComponent, SalesEditComponent],
  imports: [
    RouterModule.forRoot(routes),
    BrowserModule,
    FormsModule,
    CommonModule, 
  ],
  exports: [RouterModule],
  bootstrap: [SalesListComponent], 
})
export class AppRoutingModule { }
