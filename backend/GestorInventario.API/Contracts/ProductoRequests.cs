namespace GestorInventario.API.Contracts;

public sealed record CrearProductoRequest(string Nombre, string? Descripcion, decimal Precio,
    int Stock, int StockMinimo, int CategoriaId);

public sealed record ActualizarProductoRequest(string Nombre, string? Descripcion, decimal Precio,
    int Stock, int StockMinimo, int CategoriaId);

/// <summary>Tipo: "ENTRADA" o "SALIDA".</summary>
public sealed record AjustarStockRequest(int Cantidad, string Tipo);
