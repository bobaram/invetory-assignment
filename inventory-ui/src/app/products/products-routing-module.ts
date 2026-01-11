import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ProductList } from './product-list/product-list';
import { ProductDetail } from './product-detail/product-detail';
import { ProductForm } from './product-form/product-form';
import { authGuard } from '../core/guards/auth-guard';

const routes: Routes = [
  { 
    path: '', 
    component: ProductList,
    canActivate: [authGuard]
  },
  { 
    path: 'create', 
    component: ProductForm,
    canActivate: [authGuard]
  },
  { 
    path: ':id', 
    component: ProductDetail,
    canActivate: [authGuard]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ProductsRoutingModule { }
