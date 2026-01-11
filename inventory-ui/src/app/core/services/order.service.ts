import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface TransferOrderRequest {
  productCode: string;
  sourceWarehouseCode: string;
  destinationWarehouseCode: string;
  quantity: number;
}

@Injectable({
  providedIn: 'root'
})
export class OrderService {
  private readonly API_URL = 'http://localhost:8080/api/orders';

  constructor(private http: HttpClient) {}

  transferStock(request: TransferOrderRequest): Observable<any> {
    return this.http.post(this.API_URL, request);
  }
}
