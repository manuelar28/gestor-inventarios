import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { provideZonelessChangeDetection } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { ProductoListComponent } from './producto-list.component';

describe('ProductoListComponent', () => {
  let fixture: ComponentFixture<ProductoListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ProductoListComponent],
      providers: [
        provideZonelessChangeDetection(),
        provideRouter([]),
        provideHttpClient(),
        provideHttpClientTesting()
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(ProductoListComponent);
    fixture.detectChanges();
  });

  it('se crea el listado de productos', () => {
    expect(fixture.componentInstance).toBeTruthy();
  });

  it('muestra el título de la página', () => {
    const titulo = (fixture.nativeElement as HTMLElement).querySelector('.producto-list__titulo');
    expect(titulo?.textContent).toContain('Productos');
  });
});
