import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TransferStockComponent } from './transfer-stock/transfer-stock.component';
import { authGuard } from '../core/guards/auth-guard';

const routes: Routes = [
  { 
    path: '', 
    component: TransferStockComponent,
    canActivate: [authGuard]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class OrdersRoutingModule { }
