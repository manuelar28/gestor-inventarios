namespace GestorInventario.Domain.Exceptions;

/// <summary>Se lanza cuando se incumple una regla de negocio (validaciones del dominio).</summary>
public sealed class DominioException(string mensaje) : Exception(mensaje);
