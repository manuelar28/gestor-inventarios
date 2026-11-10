import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { AjusteStock, Categoria, Producto, ProductoPayload } from '../models/producto.model';

@Injectable({ providedIn: 'root' })
export class ProductosApiService {
  private readonly http = inject(HttpClient);
  private readonly productosUrl = `${environment.apiUrl}/productos`;
  private readonly categoriasUrl = `${environment.apiUrl}/categorias`;

  listar(): Observable<Producto[]> {
    return this.http.get<Producto[]>(this.productosUrl);
  }

  stockBajo(): Observable<Producto[]> {
    return this.http.get<Producto[]>(`${this.productosUrl}/stock-bajo`);
  }

  obtener(id: number): Observable<Producto> {
    return this.http.get<Producto>(`${this.productosUrl}/${id}`);
  }

  crear(payload: ProductoPayload): Observable<Producto> {
    return this.http.post<Producto>(this.productosUrl, payload);
  }

  actualizar(id: number, payload: ProductoPayload): Observable<Producto> {
    return this.http.put<Producto>(`${this.productosUrl}/${id}`, payload);
  }

  eliminar(id: number): Observable<void> {
    return this.http.delete<void>(`${this.productosUrl}/${id}`);
  }

  ajustarStock(id: number, ajuste: AjusteStock): Observable<Producto> {
    return this.http.patch<Producto>(`${this.productosUrl}/${id}/stock`, ajuste);
  }

  categorias(): Observable<Categoria[]> {
    return this.http.get<Categoria[]>(this.categoriasUrl);
  }
}
