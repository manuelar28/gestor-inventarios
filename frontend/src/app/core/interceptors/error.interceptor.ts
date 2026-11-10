import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { NotificacionService } from '../services/notificacion.service';

/** La API responde los errores con el formato { "error": "mensaje" }. */
function extraerMensaje(error: HttpErrorResponse): string {
  if (error.status === 0) {
    return 'No fue posible conectar con la API. Verifica que esté en ejecución.';
  }
  const cuerpo: unknown = error.error;
  if (typeof cuerpo === 'object' && cuerpo !== null && 'error' in cuerpo && typeof cuerpo.error === 'string') {
    return cuerpo.error;
  }
  return 'Ocurrió un error inesperado. Intenta nuevamente.';
}

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const notificaciones = inject(NotificacionService);
  return next(req).pipe(
    catchError((error: unknown) => {
      if (error instanceof HttpErrorResponse) {
        notificaciones.error(extraerMensaje(error));
      }
      return throwError(() => error);
    })
  );
};
