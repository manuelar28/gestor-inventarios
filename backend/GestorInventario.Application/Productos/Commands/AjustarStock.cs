using GestorInventario.Application.Abstractions;
using GestorInventario.Application.Exceptions;
using GestorInventario.Domain.Enums;
using GestorInventario.Domain.Exceptions;
using MediatR;

namespace GestorInventario.Application.Productos.Commands;

public sealed record AjustarStockCommand(int Id, int Cantidad, string Tipo) : IRequest<ProductoDto>;

public sealed class AjustarStockHandler(IProductoRepository productos)
    : IRequestHandler<AjustarStockCommand, ProductoDto>
{
    public async Task<ProductoDto> Handle(AjustarStockCommand request, CancellationToken ct)
    {
        var tipo = request.Tipo?.Trim().ToUpperInvariant() switch
        {
            "ENTRADA" => TipoMovimiento.Entrada,
            "SALIDA" => TipoMovimiento.Salida,
            _ => throw new DominioException("El tipo debe ser ENTRADA o SALIDA.")
        };

        var producto = await productos.ObtenerAsync(request.Id, ct)
            ?? throw new NoEncontradoException(Mensajes.ProductoNoEncontrado);

        producto.AjustarStock(tipo, request.Cantidad);
        await productos.ActualizarAsync(producto, ct);
        return ProductoMapping.ToDto(producto);
    }
}
