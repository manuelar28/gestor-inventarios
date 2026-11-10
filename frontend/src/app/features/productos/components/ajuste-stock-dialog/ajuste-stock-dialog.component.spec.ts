import { provideZonelessChangeDetection } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { Producto } from '../../models/producto.model';
import { AjusteStockDialogComponent } from './ajuste-stock-dialog.component';

describe('AjusteStockDialogComponent', () => {
  const producto: Producto = {
    id: 1,
    nombre: 'Mouse inalámbrico',
    descripcion: null,
    precio: 45000,
    stock: 10,
    stockMinimo: 5,
    categoriaId: 1,
    categoriaNombre: 'Electrónica',
    fechaCreacion: '2026-01-01T00:00:00',
    tieneStockBajo: false
  };

  let fixture: ComponentFixture<AjusteStockDialogComponent>;
  let component: AjusteStockDialogComponent;
  let dialogRef: jasmine.SpyObj<MatDialogRef<AjusteStockDialogComponent>>;

  beforeEach(async () => {
    dialogRef = jasmine.createSpyObj<MatDialogRef<AjusteStockDialogComponent>>('MatDialogRef', ['close']);
    await TestBed.configureTestingModule({
      imports: [AjusteStockDialogComponent],
      providers: [
        provideZonelessChangeDetection(),
        { provide: MAT_DIALOG_DATA, useValue: producto },
        { provide: MatDialogRef, useValue: dialogRef }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(AjusteStockDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('no cierra el diálogo cuando la cantidad no es válida', () => {
    component.form.controls.cantidad.setValue(0);
    component.aplicar();
    expect(dialogRef.close).not.toHaveBeenCalled();
  });

  it('cierra el diálogo con el ajuste cuando el formulario es válido', () => {
    component.form.setValue({ tipo: 'SALIDA', cantidad: 3 });
    component.aplicar();
    expect(dialogRef.close).toHaveBeenCalledWith({ tipo: 'SALIDA', cantidad: 3 });
  });
});
