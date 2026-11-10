using GestorInventario.Domain.Enums;
using GestorInventario.Domain.Exceptions;

namespace GestorInventario.Domain.Entities;

public sealed class Producto
{
    public const int LongitudMaximaNombre = 150;
    public const int LongitudMaximaDescripcion = 500;

    private Producto() { }

    public int Id { get; private set; }
    public string Nombre { get; private set; } = string.Empty;
    public string? Descripcion { get; private set; }
    public decimal Precio { get; private set; }
    public int Stock { get; private set; }
    public int StockMinimo { get; private set; }
    public int CategoriaId { get; private set; }
    public DateTime FechaCreacion { get; private set; } = DateTime.UtcNow;
    public Categoria? Categoria { get; private set; }

    /// <summary>Regla: hay stock bajo cuando Stock &lt; StockMinimo.</summary>
    public bool TieneStockBajo => Stock < StockMinimo;

    public static Producto Crear(string nombre, string? descripcion, decimal precio,
        int stock, int stockMinimo, int categoriaId)
    {
        var producto = new Producto();
        producto.Actualizar(nombre, descripcion, precio, stock, stockMinimo, categoriaId);
        return producto;
    }

    public void Actualizar(string nombre, string? descripcion, decimal precio,
        int stock, int stockMinimo, int categoriaId)
    {
        Validar(nombre, descripcion, precio, stock, stockMinimo, categoriaId);

        Nombre = nombre.Trim();
        Descripcion = string.IsNullOrWhiteSpace(descripcion) ? null : descripcion.Trim();
        Precio = precio;
        Stock = stock;
        StockMinimo = stockMinimo;
        CategoriaId = categoriaId;
    }

    public void AjustarStock(TipoMovimiento tipo, int cantidad)
    {
        if (cantidad <= 0)
            throw new DominioException("La cantidad debe ser mayor que cero.");

        switch (tipo)
        {
            case TipoMovimiento.Entrada:
                if (cantidad > int.MaxValue - Stock)
                    throw new DominioException("La cantidad supera el stock máximo permitido.");
                Stock += cantidad;
                break;

            case TipoMovimiento.Salida:
                if (cantidad > Stock)
                    throw new DominioException("No hay stock suficiente para realizar la salida.");
                Stock -= cantidad;
                break;

            default:
                throw new DominioException("El tipo de movimiento no es válido.");
        }
    }

    private static void Validar(string nombre, string? descripcion, decimal precio,
        int stock, int stockMinimo, int categoriaId)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new DominioException("El nombre es obligatorio.");
        if (nombre.Trim().Length > LongitudMaximaNombre)
            throw new DominioException($"El nombre no puede superar {LongitudMaximaNombre} caracteres.");
        if (descripcion is not null && descripcion.Trim().Length > LongitudMaximaDescripcion)
            throw new DominioException($"La descripción no puede superar {LongitudMaximaDescripcion} caracteres.");
        if (precio <= 0)
            throw new DominioException("El precio debe ser mayor que cero.");
        if (stock < 0)
            throw new DominioException("El stock no puede ser negativo.");
        if (stockMinimo < 0)
            throw new DominioException("El stock mínimo no puede ser negativo.");
        if (categoriaId <= 0)
            throw new DominioException("Debes seleccionar una categoría.");
    }
}
