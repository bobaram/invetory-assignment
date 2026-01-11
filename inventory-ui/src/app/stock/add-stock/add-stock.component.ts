import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { StockService } from '../../core/services/stock.service';
import { ProductService, Product } from '../../core/services/product.service';
import { WarehouseService, Warehouse } from '../../core/services/warehouse.service';

@Component({
  selector: 'app-add-stock',
  standalone: false,
  templateUrl: './add-stock.component.html',
  styleUrls: ['./add-stock.component.css']
})
export class AddStockComponent implements OnInit {
  stockForm: FormGroup;
  products: Product[] = [];
  warehouses: Warehouse[] = [];
  errorMessage: string = '';
  successMessage: string = '';
  loading: boolean = false;

  constructor(
    private fb: FormBuilder,
    private stockService: StockService,
    private productService: ProductService,
    private warehouseService: WarehouseService,
    private router: Router
  ) {
    this.stockForm = this.fb.group({
      productCode: ['', [Validators.required]],
      warehouseCode: ['', [Validators.required]],
      quantity: ['', [Validators.required, Validators.min(1)]]
    });
  }

  ngOnInit(): void {
    this.loadProducts();
    this.loadWarehouses();
  }

  loadProducts(): void {
    this.productService.getProducts().subscribe({
      next: (data) => this.products = data,
      error: () => this.errorMessage = 'Failed to load products'
    });
  }

  loadWarehouses(): void {
    this.warehouseService.getWarehouses().subscribe({
      next: (data) => this.warehouses = data,
      error: () => this.errorMessage = 'Failed to load warehouses'
    });
  }

  onSubmit(): void {
    if (this.stockForm.valid) {
      this.loading = true;
      this.errorMessage = '';
      this.successMessage = '';

      this.stockService.addStock(this.stockForm.value).subscribe({
        next: () => {
          this.successMessage = 'Stock added successfully!';
          setTimeout(() => {
            this.router.navigate(['/stock']);
          }, 1500);
        },
        error: (error) => {
          this.errorMessage = error.error?.message || 'Failed to add stock';
          this.loading = false;
        },
        complete: () => {
          this.loading = false;
        }
      });
    }
  }

  cancel(): void {
    this.router.navigate(['/stock']);
  }
}
