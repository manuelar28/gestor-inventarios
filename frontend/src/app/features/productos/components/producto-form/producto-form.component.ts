import { ChangeDetectionStrategy, Component, computed, inject, input, OnInit, signal } from '@angular/core';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { Router, RouterLink } from '@angular/router';
import { NotificacionService } from '../../../../core/services/notificacion.service';
import { ProductoPayload } from '../../models/producto.model';
import { ProductosStore } from '../../services/productos.store';

@Component({
  selector: 'app-producto-form',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink, MatButtonModule, MatFormFieldModule, MatInputModule, MatSelectModule],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './producto-form.component.html',
  styleUrl: './producto-form.component.scss'
})
export class ProductoFormComponent implements OnInit {
  /** Parámetro de ruta ":id" (enlazado con withComponentInputBinding). Sin valor = crear. */
  readonly id = input<string>();

  protected readonly store = inject(ProductosStore);
  private readonly router = inject(Router);
  private readonly notificaciones = inject(NotificacionService);
  private readonly fb = inject(NonNullableFormBuilder);

  protected readonly guardando = signal(false);
  protected readonly esEdicion = computed(() => this.id() !== undefined);
  protected readonly titulo = computed(() => (this.esEdicion() ? 'Editar producto' : 'Nuevo producto'));
  protected readonly textoGuardar = computed(() => (this.esEdicion() ? 'Guardar cambios' : 'Guardar producto'));

  readonly form = this.fb.group({
    nombre: ['', [Validators.required, Validators.maxLength(150)]],
    descripcion: ['', [Validators.maxLength(500)]],
    precio: [0, [Validators.required, Validators.min(0.01)]],
    stock: [0, [Validators.required, Validators.min(0)]],
    stockMinimo: [5, [Validators.required, Validators.min(0)]],
    categoriaId: this.fb.control<number | null>(null, [Validators.required])
  });

  ngOnInit(): void {
    if (this.store.categorias().length === 0) {
      this.store.cargarCategorias();
    }
    const id = this.id();
    if (id !== undefined) {
      this.cargarProducto(Number(id));
    }
  }

  protected guardar(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const valores = this.form.getRawValue();
    if (valores.categoriaId === null) {
      return;
    }

    const payload: ProductoPayload = {
      nombre: valores.nombre.trim(),
      descripcion: valores.descripcion.trim() || null,
      precio: valores.precio,
      stock: valores.stock,
      stockMinimo: valores.stockMinimo,
      categoriaId: valores.categoriaId
    };

    const id = this.id();
    const peticion = id === undefined ? this.store.crear(payload) : this.store.actualizar(Number(id), payload);

    this.guardando.set(true);
    peticion.subscribe({
      next: () => {
        this.notificaciones.exito(id === undefined ? 'Producto creado.' : 'Producto actualizado.');
        void this.router.navigate(['/productos']);
      },
      error: () => this.guardando.set(false)
    });
  }

  private cargarProducto(id: number): void {
    this.store.obtener(id).subscribe({
      next: producto =>
        this.form.patchValue({
          nombre: producto.nombre,
          descripcion: producto.descripcion ?? '',
          precio: producto.precio,
          stock: producto.stock,
          stockMinimo: producto.stockMinimo,
          categoriaId: producto.categoriaId
        }),
      error: () => void this.router.navigate(['/productos'])
    });
  }
}
