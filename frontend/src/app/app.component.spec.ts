import { provideZonelessChangeDetection } from '@angular/core';
import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { AppComponent } from './app.component';

describe('AppComponent', () => {
  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AppComponent],
      providers: [provideZonelessChangeDetection(), provideRouter([])]
    }).compileComponents();
  });

  it('se crea el contenedor principal', () => {
    const fixture = TestBed.createComponent(AppComponent);
    expect(fixture.componentInstance).toBeTruthy();
  });

  it('muestra el nombre de la aplicación', () => {
    const fixture = TestBed.createComponent(AppComponent);
    fixture.detectChanges();
    const marca = (fixture.nativeElement as HTMLElement).querySelector('.app-header__marca');
    expect(marca?.textContent).toContain('Gestor de Inventario');
  });
});
