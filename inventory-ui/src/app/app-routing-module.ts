import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { authGuard } from './core/guards/auth-guard';

const routes: Routes = [
  { 
    path: 'auth', 
    loadChildren: () => import('./auth/auth-module').then(m => m.AuthModule)
  },
  { 
    path: 'products', 
    loadChildren: () => import('./products/products-module').then(m => m.ProductsModule),
    canActivate: [authGuard]
  },
  { 
    path: 'warehouses', 
    loadChildren: () => import('./warehouses/warehouses-module').then(m => m.WarehousesModule),
    canActivate: [authGuard]
  },
  { 
    path: 'stock', 
    loadChildren: () => import('./stock/stock-module').then(m => m.StockModule),
    canActivate: [authGuard]
  },
  { 
    path: 'orders', 
    loadChildren: () => import('./orders/orders-module').then(m => m.OrdersModule),
    canActivate: [authGuard]
  },
  { path: '', redirectTo: '/products', pathMatch: 'full' },
  { path: '**', redirectTo: '/products' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
