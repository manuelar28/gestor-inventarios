namespace GestorInventario.Application.Exceptions;

/// <summary>Se lanza cuando un recurso solicitado no existe (se traduce a HTTP 404).</summary>
public sealed class NoEncontradoException(string mensaje) : Exception(mensaje);
