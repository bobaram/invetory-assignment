import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { StockListComponent } from './stock-list/stock-list.component';
import { AddStockComponent } from './add-stock/add-stock.component';
import { authGuard } from '../core/guards/auth-guard';

const routes: Routes = [
  { 
    path: '', 
    component: StockListComponent,
    canActivate: [authGuard]
  },
  { 
    path: 'add', 
    component: AddStockComponent,
    canActivate: [authGuard]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class StockRoutingModule { }
