import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';

import { WarehousesRoutingModule } from './warehouses-routing-module';
import { WarehouseForm } from './warehouse-form/warehouse-form';
import { WarehouseListComponent } from './warehouse-list/warehouse-list.component';


@NgModule({
  declarations: [
    WarehouseForm,
    WarehouseListComponent
  ],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    WarehousesRoutingModule
  ]
})
export class WarehousesModule { }
