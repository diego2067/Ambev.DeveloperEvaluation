import { Component, OnInit } from '@angular/core';
import { SalesService } from '../../services/sales.service';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-sales-list',
  standalone: true,
  imports: [CommonModule, FormsModule], 
  templateUrl: './sales-list.component.html',
  styleUrls: ['./sales-list.component.css'],
})
export class SalesListComponent implements OnInit {
  sales: any[] = [];
  selectedSale: any = { customer: '', branch: '', items: [] };
  products: any[] = [];

  constructor(private salesService: SalesService, private modalService: NgbModal) { }

  ngOnInit(): void {
    this.loadSales();
  }

  loadSales(): void {
    this.salesService.listSales().subscribe({
      next: (data) => (this.sales = data),
      error: (err) => console.error('Erro ao carregar vendas:', err),
    });
  }

  editSale(saleId: string, modal: any): void {
    this.salesService.getSaleById(saleId).subscribe({
      next: (data) => {
        this.selectedSale = data;
        this.modalService.open(modal);
      },
      error: (err) => console.error('Erro ao carregar venda:', err),
    });
  }

  updateSale(modal: any): void {
    this.salesService.updateSale(this.selectedSale.id).subscribe({
      next: () => {
        alert('Venda atualizada com sucesso!');
        this.loadSales();
        modal.close();
      },
      error: (err) => console.error('Erro ao atualizar venda:', err),
    });
  }

  showProducts(saleId: string, modal: any): void {
    this.salesService.getSaleById(saleId).subscribe({
      next: (data) => {
        if (data && data.items) {
          this.products = data.items; // Pega apenas os itens da venda
          this.modalService.open(modal); // Abre o modal
        } else {
          alert('Nenhum item encontrado para esta venda.');
        }
      },
      error: (err) => {
        console.error('Erro ao carregar itens da venda:', err);
        alert('Erro ao carregar os itens da venda.');
      },
    });
  }

  // Exclui uma venda e atualiza o status na tela
  deleteSale(saleId: string): void {
    if (confirm('Tem certeza que deseja excluir esta venda?')) {
      this.salesService.deleteSale(saleId).subscribe({
        next: () => {
          alert('Venda cancelada com sucesso!');
          this.loadSales();
        },
        error: (err) => console.error('Erro ao cancelar venda:', err),
      });
    }
  }
}
