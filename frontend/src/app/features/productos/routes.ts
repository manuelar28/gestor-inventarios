import { Routes } from '@angular/router';

export const PRODUCTOS_ROUTES: Routes = [
  {
    path: '',
    title: 'Productos',
    loadComponent: () =>
      import('./components/producto-list/producto-list.component').then(m => m.ProductoListComponent)
  },
  {
    path: 'nuevo',
    title: 'Nuevo producto',
    loadComponent: () =>
      import('./components/producto-form/producto-form.component').then(m => m.ProductoFormComponent)
  },
  {
    path: ':id/editar',
    title: 'Editar producto',
    loadComponent: () =>
      import('./components/producto-form/producto-form.component').then(m => m.ProductoFormComponent)
  }
];
