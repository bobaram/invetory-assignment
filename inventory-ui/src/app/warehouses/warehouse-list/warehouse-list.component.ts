import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { WarehouseService, Warehouse } from '../../core/services/warehouse.service';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-warehouse-list',
  standalone: false,
  templateUrl: './warehouse-list.component.html',
  styleUrls: ['./warehouse-list.component.css']
})
export class WarehouseListComponent implements OnInit {
  warehouses: Warehouse[] = [];
  loading: boolean = false;
  errorMessage: string = '';

  constructor(
    private warehouseService: WarehouseService,
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadWarehouses();
  }

  loadWarehouses(): void {
    this.loading = true;
    this.errorMessage = '';

    this.warehouseService.getWarehouses().subscribe({
      next: (data) => {
        this.warehouses = data;
        this.loading = false;
      },
      error: (error) => {
        this.errorMessage = error.error?.message || 'Failed to load warehouses';
        this.loading = false;
      }
    });
  }

  createWarehouse(): void {
    this.router.navigate(['/warehouses/create']);
  }

  logout(): void {
    this.authService.logout();
  }
}
