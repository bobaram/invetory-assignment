import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { WarehouseService, Warehouse } from '../../core/services/warehouse.service';

@Component({
  selector: 'app-warehouse-form',
  standalone: false,
  templateUrl: './warehouse-form.html',
  styleUrl: './warehouse-form.css',
})
export class WarehouseForm implements OnInit {
  warehouseForm: FormGroup;
  errorMessage: string = '';
  successMessage: string = '';
  loading: boolean = false;

  constructor(
    private fb: FormBuilder,
    private warehouseService: WarehouseService,
    private router: Router
  ) {
    this.warehouseForm = this.fb.group({
      code: ['', [Validators.required]],
      name: ['', [Validators.required]]
    });
  }

  ngOnInit(): void {}

  onSubmit(): void {
    if (this.warehouseForm.valid) {
      this.loading = true;
      this.errorMessage = '';
      this.successMessage = '';

      this.warehouseService.createWarehouse(this.warehouseForm.value).subscribe({
        next: () => {
          this.successMessage = 'Warehouse created successfully!';
          setTimeout(() => {
            this.router.navigate(['/warehouses']);
          }, 1500);
        },
        error: (error) => {
          this.errorMessage = error.error?.message || 'Failed to create warehouse';
          this.loading = false;
        },
        complete: () => {
          this.loading = false;
        }
      });
    }
  }

  cancel(): void {
    this.router.navigate(['/warehouses']);
  }
}
