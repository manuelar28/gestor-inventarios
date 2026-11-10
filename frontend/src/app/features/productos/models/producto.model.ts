export interface Producto {
  id: number;
  nombre: string;
  descripcion: string | null;
  precio: number;
  stock: number;
  stockMinimo: number;
  categoriaId: number;
  categoriaNombre: string;
  fechaCreacion: string;
  tieneStockBajo: boolean;
}

export interface Categoria {
  id: number;
  nombre: string;
}

export interface ProductoPayload {
  nombre: string;
  descripcion: string | null;
  precio: number;
  stock: number;
  stockMinimo: number;
  categoriaId: number;
}

export type TipoMovimiento = 'ENTRADA' | 'SALIDA';

export interface AjusteStock {
  tipo: TipoMovimiento;
  cantidad: number;
}

export type EstadoStock = 'todos' | 'bajo' | 'normal';
export type FiltroCategoria = number | 'todas';
