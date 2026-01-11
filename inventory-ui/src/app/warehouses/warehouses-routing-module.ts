import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { WarehouseListComponent } from './warehouse-list/warehouse-list.component';
import { WarehouseForm } from './warehouse-form/warehouse-form';
import { authGuard } from '../core/guards/auth-guard';

const routes: Routes = [
  { 
    path: '', 
    component: WarehouseListComponent,
    canActivate: [authGuard]
  },
  { 
    path: 'create', 
    component: WarehouseForm,
    canActivate: [authGuard]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class WarehousesRoutingModule { }
