import { Component } from '@angular/core';
import { SalesListComponent } from './components/sales-list/sales-list.component';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [SalesListComponent, CommonModule], 
  template: `<app-sales-list></app-sales-list>`,
})
export class AppComponent { }
