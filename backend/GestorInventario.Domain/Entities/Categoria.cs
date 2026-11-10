using GestorInventario.Domain.Exceptions;

namespace GestorInventario.Domain.Entities;

public sealed class Categoria
{
    public const int LongitudMaximaNombre = 100;

    private Categoria() { }

    public int Id { get; private set; }
    public string Nombre { get; private set; } = string.Empty;
    public bool Activo { get; private set; } = true;

    public static Categoria Crear(string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new DominioException("El nombre de la categoría es obligatorio.");
        if (nombre.Trim().Length > LongitudMaximaNombre)
            throw new DominioException($"El nombre de la categoría no puede superar {LongitudMaximaNombre} caracteres.");

        return new Categoria { Nombre = nombre.Trim() };
    }
}
