import { CurrencyPipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject, OnInit } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatSelectModule } from '@angular/material/select';
import { MatTableModule } from '@angular/material/table';
import { RouterLink } from '@angular/router';
import { filter, switchMap } from 'rxjs';
import { NotificacionService } from '../../../../core/services/notificacion.service';
import {
  ConfirmDialogComponent,
  ConfirmDialogData
} from '../../../../shared/components/confirm-dialog/confirm-dialog.component';
import { AjusteStock, EstadoStock, FiltroCategoria, Producto } from '../../models/producto.model';
import { ProductosStore } from '../../services/productos.store';
import { AjusteStockDialogComponent } from '../ajuste-stock-dialog/ajuste-stock-dialog.component';

@Component({
  selector: 'app-producto-list',
  standalone: true,
  imports: [
    CurrencyPipe,
    RouterLink,
    MatButtonModule,
    MatFormFieldModule,
    MatProgressBarModule,
    MatSelectModule,
    MatTableModule
  ],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './producto-list.component.html',
  styleUrl: './producto-list.component.scss'
})
export class ProductoListComponent implements OnInit {
  protected readonly store = inject(ProductosStore);
  private readonly dialog = inject(MatDialog);
  private readonly notificaciones = inject(NotificacionService);

  protected readonly columnas: string[] = ['nombre', 'categoria', 'precio', 'stock', 'stockMinimo', 'acciones'];

  ngOnInit(): void {
    this.store.cargarTodo();
  }

  protected cambiarCategoria(categoria: FiltroCategoria): void {
    this.store.categoriaFiltro.set(categoria);
  }

  protected cambiarEstado(estado: EstadoStock): void {
    this.store.estadoFiltro.set(estado);
  }

  protected ajustarStock(producto: Producto): void {
    this.dialog
      .open<AjusteStockDialogComponent, Producto, AjusteStock>(AjusteStockDialogComponent, {
        data: producto,
        width: '420px'
      })
      .afterClosed()
      .pipe(
        filter((ajuste): ajuste is AjusteStock => ajuste !== undefined),
        switchMap(ajuste => this.store.ajustarStock(producto.id, ajuste))
      )
      .subscribe({
        next: () => this.notificaciones.exito('Stock actualizado.'),
        error: () => undefined
      });
  }

  protected eliminar(producto: Producto): void {
    this.dialog
      .open<ConfirmDialogComponent, ConfirmDialogData, boolean>(ConfirmDialogComponent, {
        data: {
          titulo: 'Eliminar producto',
          mensaje: `¿Seguro que deseas eliminar "${producto.nombre}"? Esta acción no se puede deshacer.`,
          textoConfirmar: 'Eliminar'
        }
      })
      .afterClosed()
      .pipe(
        filter(confirmado => confirmado === true),
        switchMap(() => this.store.eliminar(producto.id))
      )
      .subscribe({
        next: () => this.notificaciones.exito('Producto eliminado.'),
        error: () => undefined
      });
  }
}
