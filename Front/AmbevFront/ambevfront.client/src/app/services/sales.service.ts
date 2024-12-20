import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class SalesService {
  private apiUrl = 'https://localhost:7181';

  constructor(private http: HttpClient) { }

  listSales(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/api/sales/list`);
  }

  createSale(sale: any): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/create`, sale);
  }

  getProductsBySaleId(saleId: string): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/${saleId}/products`);
  }

  getSaleById(saleId: string): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/api/sales/${saleId}`);
  }

  updateSale(sale: any): Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/${sale.Id}`, sale);
  }

  deleteSale(saleId: string): Observable<any> {
    return this.http.delete<any>(`${this.apiUrl}/${saleId}`);
  }
}
