import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Warehouse {
  id?: number;
  code: string;
  name: string;
}

@Injectable({
  providedIn: 'root'
})
export class WarehouseService {
  private readonly API_URL = 'http://localhost:8080/api/warehouses';

  constructor(private http: HttpClient) {}

  getWarehouses(): Observable<Warehouse[]> {
    return this.http.get<Warehouse[]>(this.API_URL);
  }

  createWarehouse(warehouse: Warehouse): Observable<Warehouse> {
    return this.http.post<Warehouse>(this.API_URL, warehouse);
  }
}
