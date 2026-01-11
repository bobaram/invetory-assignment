import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';

import { StockRoutingModule } from './stock-routing-module';
import { StockListComponent } from './stock-list/stock-list.component';
import { AddStockComponent } from './add-stock/add-stock.component';


@NgModule({
  declarations: [
    StockListComponent,
    AddStockComponent
  ],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    StockRoutingModule
  ]
})
export class StockModule { }
