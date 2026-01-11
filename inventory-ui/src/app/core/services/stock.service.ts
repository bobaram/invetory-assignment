import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface StockItem {
  productCode: string;
  warehouseCode: string;
  quantity: number;
  productDescription?: string;
  warehouseName?: string;
}

export interface AddStockRequest {
  productCode: string;
  warehouseCode: string;
  quantity: number;
}

@Injectable({
  providedIn: 'root'
})
export class StockService {
  private readonly API_URL = 'http://localhost:8080/api/stock';

  constructor(private http: HttpClient) {}

  addStock(request: AddStockRequest): Observable<any> {
    return this.http.post(this.API_URL, request);
  }

  getStockByProduct(productCode: string): Observable<StockItem[]> {
    const params = new HttpParams().set('productCode', productCode);
    return this.http.get<StockItem[]>(this.API_URL, { params });
  }

  getStockByWarehouse(warehouseCode: string): Observable<StockItem[]> {
    const params = new HttpParams().set('warehouseCode', warehouseCode);
    return this.http.get<StockItem[]>(this.API_URL, { params });
  }

  getAllStock(): Observable<StockItem[]> {
    return this.http.get<StockItem[]>(this.API_URL);
  }
}
