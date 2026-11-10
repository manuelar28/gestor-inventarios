import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { provideZonelessChangeDetection } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { ProductoFormComponent } from './producto-form.component';

describe('ProductoFormComponent', () => {
  let fixture: ComponentFixture<ProductoFormComponent>;
  let component: ProductoFormComponent;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ProductoFormComponent],
      providers: [
        provideZonelessChangeDetection(),
        provideRouter([]),
        provideHttpClient(),
        provideHttpClientTesting()
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(ProductoFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('inicia con el formulario inválido (nombre y categoría obligatorios)', () => {
    expect(component.form.invalid).toBeTrue();
    expect(component.form.controls.nombre.hasError('required')).toBeTrue();
    expect(component.form.controls.categoriaId.hasError('required')).toBeTrue();
  });

  it('rechaza un precio menor o igual a cero', () => {
    component.form.controls.precio.setValue(0);
    expect(component.form.controls.precio.hasError('min')).toBeTrue();
  });

  it('rechaza un stock negativo', () => {
    component.form.controls.stock.setValue(-1);
    expect(component.form.controls.stock.hasError('min')).toBeTrue();
  });

  it('es válido con datos correctos', () => {
    component.form.patchValue({
      nombre: 'Mouse inalámbrico',
      precio: 45000,
      stock: 10,
      stockMinimo: 5,
      categoriaId: 1
    });
    expect(component.form.valid).toBeTrue();
  });
});
