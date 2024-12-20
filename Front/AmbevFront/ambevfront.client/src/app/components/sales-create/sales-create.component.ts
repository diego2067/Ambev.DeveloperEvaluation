import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-sales-create',
  standalone: true, 
  imports: [CommonModule, FormsModule], 
  templateUrl: './sales-create.component.html',
  styleUrls: ['./sales-create.component.css'],
})
export class SalesCreateComponent {
  newSale = {
    saleNumber: '',
    customer: '',
    branch: '',
    items: [{ product: '', quantity: 0, unitPrice: 0 }],
  };

  addItem(): void {
    this.newSale.items.push({ product: '', quantity: 0, unitPrice: 0 });
  }

  saveSale(): void {
    console.log('Nova venda salva:', this.newSale); 
   
  }
}
