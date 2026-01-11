import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { OrderService } from '../../core/services/order.service';
import { ProductService, Product } from '../../core/services/product.service';
import { WarehouseService, Warehouse } from '../../core/services/warehouse.service';

@Component({
  selector: 'app-transfer-stock',
  standalone: false,
  templateUrl: './transfer-stock.component.html',
  styleUrls: ['./transfer-stock.component.css']
})
export class TransferStockComponent implements OnInit {
  transferForm: FormGroup;
  products: Product[] = [];
  warehouses: Warehouse[] = [];
  errorMessage: string = '';
  successMessage: string = '';
  loading: boolean = false;

  constructor(
    private fb: FormBuilder,
    private orderService: OrderService,
    private productService: ProductService,
    private warehouseService: WarehouseService,
    private router: Router
  ) {
    this.transferForm = this.fb.group({
      productCode: ['', [Validators.required]],
      sourceWarehouseCode: ['', [Validators.required]],
      destinationWarehouseCode: ['', [Validators.required]],
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
    if (this.transferForm.valid) {
      const { sourceWarehouseCode, destinationWarehouseCode } = this.transferForm.value;
      
      if (sourceWarehouseCode === destinationWarehouseCode) {
        this.errorMessage = 'Source and destination warehouses cannot be the same';
        return;
      }

      this.loading = true;
      this.errorMessage = '';
      this.successMessage = '';

      this.orderService.transferStock(this.transferForm.value).subscribe({
        next: () => {
          this.successMessage = 'Stock transferred successfully!';
          setTimeout(() => {
            this.router.navigate(['/stock']);
          }, 1500);
        },
        error: (error) => {
          this.errorMessage = error.error?.message || 'Failed to transfer stock';
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
