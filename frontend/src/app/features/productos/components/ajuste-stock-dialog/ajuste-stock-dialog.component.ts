import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { AjusteStock, Producto, TipoMovimiento } from '../../models/producto.model';

@Component({
  selector: 'app-ajuste-stock-dialog',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatDialogModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule
  ],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './ajuste-stock-dialog.component.html',
  styleUrl: './ajuste-stock-dialog.component.scss'
})
export class AjusteStockDialogComponent {
  protected readonly producto = inject<Producto>(MAT_DIALOG_DATA);
  private readonly dialogRef = inject<MatDialogRef<AjusteStockDialogComponent, AjusteStock>>(MatDialogRef);
  private readonly fb = inject(NonNullableFormBuilder);

  readonly form = this.fb.group({
    tipo: ['ENTRADA' as TipoMovimiento, [Validators.required]],
    cantidad: [1, [Validators.required, Validators.min(1), Validators.pattern(/^\d+$/)]]
  });

  aplicar(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.dialogRef.close(this.form.getRawValue());
  }
}
