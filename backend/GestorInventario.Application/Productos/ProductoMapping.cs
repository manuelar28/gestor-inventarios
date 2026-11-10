using GestorInventario.Domain.Entities;

namespace GestorInventario.Application.Productos;

internal static class ProductoMapping
{
    public static ProductoDto ToDto(Producto p) => new(
        p.Id, p.Nombre, p.Descripcion, p.Precio, p.Stock, p.StockMinimo,
        p.CategoriaId, p.Categoria?.Nombre ?? string.Empty, p.FechaCreacion,
        p.TieneStockBajo);
}
