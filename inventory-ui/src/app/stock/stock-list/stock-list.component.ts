import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { StockService, StockItem } from '../../core/services/stock.service';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-stock-list',
  standalone: false,
  templateUrl: './stock-list.component.html',
  styleUrls: ['./stock-list.component.css']
})
export class StockListComponent implements OnInit {
  stockItems: StockItem[] = [];
  loading: boolean = false;
  errorMessage: string = '';
  filterType: string = 'all';
  filterValue: string = '';

  constructor(
    private stockService: StockService,
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadAllStock();
  }

  loadAllStock(): void {
    this.loading = true;
    this.errorMessage = '';

    this.stockService.getAllStock().subscribe({
      next: (data) => {
        this.stockItems = data;
        this.loading = false;
      },
      error: (error) => {
        this.errorMessage = error.error?.message || 'Failed to load stock';
        this.loading = false;
      }
    });
  }

  applyFilter(): void {
    if (this.filterType === 'product' && this.filterValue) {
      this.loading = true;
      this.stockService.getStockByProduct(this.filterValue).subscribe({
        next: (data) => {
          this.stockItems = data;
          this.loading = false;
        },
        error: (error) => {
          this.errorMessage = 'Product not found or error loading stock';
          this.loading = false;
        }
      });
    } else if (this.filterType === 'warehouse' && this.filterValue) {
      this.loading = true;
      this.stockService.getStockByWarehouse(this.filterValue).subscribe({
        next: (data) => {
          this.stockItems = data;
          this.loading = false;
        },
        error: (error) => {
          this.errorMessage = 'Warehouse not found or error loading stock';
          this.loading = false;
        }
      });
    } else {
      this.loadAllStock();
    }
  }

  clearFilter(): void {
    this.filterType = 'all';
    this.filterValue = '';
    this.loadAllStock();
  }

  addStock(): void {
    this.router.navigate(['/stock/add']);
  }

  logout(): void {
    this.authService.logout();
  }
}
