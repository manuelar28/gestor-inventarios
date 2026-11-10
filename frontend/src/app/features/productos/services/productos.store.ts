import { computed, inject, Injectable, signal } from '@angular/core';
import { finalize, Observable, tap } from 'rxjs';
import {
  AjusteStock,
  Categoria,
  EstadoStock,
  FiltroCategoria,
  Producto,
  ProductoPayload
} from '../models/producto.model';
import { ProductosApiService } from './productos-api.service';

/**
 * Estado de la feature con Signals. Las actualizaciones siempre reemplazan el arreglo
 * (inmutabilidad), de modo que los componentes OnPush se repintan correctamente.
 * Los errores HTTP los notifica el interceptor global.
 */
@Injectable({ providedIn: 'root' })
export class ProductosStore {
  private readonly api = inject(ProductosApiService);

  readonly productos = signal<Producto[]>([]);
  readonly categorias = signal<Categoria[]>([]);
  readonly cargando = signal(false);
  readonly categoriaFiltro = signal<FiltroCategoria>('todas');
  readonly estadoFiltro = signal<EstadoStock>('todos');

  readonly productosFiltrados = computed(() => {
    const categoria = this.categoriaFiltro();
    const estado = this.estadoFiltro();
    return this.productos().filter(producto => {
      const coincideCategoria = categoria === 'todas' || producto.categoriaId === categoria;
      const coincideEstado = estado === 'todos' || (estado === 'bajo') === producto.tieneStockBajo;
      return coincideCategoria && coincideEstado;
    });
  });

  readonly totalStockBajo = computed(() => this.productos().filter(p => p.tieneStockBajo).length);

  cargarTodo(): void {
    this.cargarProductos();
    this.cargarCategorias();
  }

  cargarProductos(): void {
    this.cargando.set(true);
    this.api
      .listar()
      .pipe(finalize(() => this.cargando.set(false)))
      .subscribe({ next: datos => this.productos.set(datos), error: () => undefined });
  }

  cargarCategorias(): void {
    this.api.categorias().subscribe({ next: datos => this.categorias.set(datos), error: () => undefined });
  }

  obtener(id: number): Observable<Producto> {
    return this.api.obtener(id);
  }

  crear(payload: ProductoPayload): Observable<Producto> {
    return this.api.crear(payload).pipe(tap(nuevo => this.productos.update(lista => [...lista, nuevo])));
  }

  actualizar(id: number, payload: ProductoPayload): Observable<Producto> {
    return this.api.actualizar(id, payload).pipe(tap(actualizado => this.reemplazar(actualizado)));
  }

  eliminar(id: number): Observable<void> {
    return this.api.eliminar(id).pipe(tap(() => this.productos.update(lista => lista.filter(p => p.id !== id))));
  }

  ajustarStock(id: number, ajuste: AjusteStock): Observable<Producto> {
    return this.api.ajustarStock(id, ajuste).pipe(tap(actualizado => this.reemplazar(actualizado)));
  }

  private reemplazar(actualizado: Producto): void {
    this.productos.update(lista => lista.map(p => (p.id === actualizado.id ? actualizado : p)));
  }
}
