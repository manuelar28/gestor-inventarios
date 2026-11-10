using GestorInventario.Application.Exceptions;
using GestorInventario.Domain.Exceptions;

namespace GestorInventario.API.Middleware;

/// <summary>
/// Traduce las excepciones de negocio a respuestas HTTP con el formato { "error": "mensaje" }.
/// Los errores inesperados se registran y no exponen detalles internos al cliente.
/// </summary>
public sealed class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            if (context.Response.HasStarted) throw;

            var (status, mensaje) = ex switch
            {
                DominioException => (StatusCodes.Status400BadRequest, ex.Message),
                NoEncontradoException => (StatusCodes.Status404NotFound, ex.Message),
                _ => (StatusCodes.Status500InternalServerError, "Ocurrió un error inesperado. Intenta nuevamente.")
            };

            if (status == StatusCodes.Status500InternalServerError)
                logger.LogError(ex, "Error no controlado en {Metodo} {Ruta}", context.Request.Method, context.Request.Path);
            else
                logger.LogWarning("{Metodo} {Ruta} respondió {Status}: {Mensaje}",
                    context.Request.Method, context.Request.Path, status, ex.Message);

            context.Response.StatusCode = status;
            await context.Response.WriteAsJsonAsync(new { error = mensaje });
        }
    }
}
