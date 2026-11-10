import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { provideZonelessChangeDetection } from '@angular/core';
import { TestBed } from '@angular/core/testing';
import { Producto } from '../models/producto.model';
import { ProductosStore } from './productos.store';

function crearProducto(parcial: Partial<Producto>): Producto {
  return {
    id: 1,
    nombre: 'Producto',
    descripcion: null,
    precio: 1000,
    stock: 10,
    stockMinimo: 5,
    categoriaId: 1,
    categoriaNombre: 'Electrónica',
    fechaCreacion: '2026-01-01T00:00:00',
    tieneStockBajo: false,
    ...parcial
  };
}

describe('ProductosStore', () => {
  let store: ProductosStore;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideZonelessChangeDetection(), provideHttpClient(), provideHttpClientTesting()]
    });
    store = TestBed.inject(ProductosStore);
    store.productos.set([
      crearProducto({ id: 1, categoriaId: 1, tieneStockBajo: false }),
      crearProducto({ id: 2, categoriaId: 1, stock: 2, tieneStockBajo: true }),
      crearProducto({ id: 3, categoriaId: 2, stock: 1, tieneStockBajo: true })
    ]);
  });

  it('devuelve todos los productos sin filtros', () => {
    expect(store.productosFiltrados().length).toBe(3);
  });

  it('filtra por categoría', () => {
    store.categoriaFiltro.set(2);
    expect(store.productosFiltrados().map(p => p.id)).toEqual([3]);
  });

  it('filtra por stock bajo', () => {
    store.estadoFiltro.set('bajo');
    expect(store.productosFiltrados().map(p => p.id)).toEqual([2, 3]);
  });

  it('filtra por stock normal', () => {
    store.estadoFiltro.set('normal');
    expect(store.productosFiltrados().map(p => p.id)).toEqual([1]);
  });

  it('combina categoría y estado de stock', () => {
    store.categoriaFiltro.set(1);
    store.estadoFiltro.set('bajo');
    expect(store.productosFiltrados().map(p => p.id)).toEqual([2]);
  });

  it('cuenta los productos con stock bajo', () => {
    expect(store.totalStockBajo()).toBe(2);
  });
});
