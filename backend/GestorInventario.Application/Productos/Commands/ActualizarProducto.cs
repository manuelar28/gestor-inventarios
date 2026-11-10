using GestorInventario.Application.Abstractions;
using GestorInventario.Application.Exceptions;
using GestorInventario.Domain.Exceptions;
using MediatR;

namespace GestorInventario.Application.Productos.Commands;

public sealed record ActualizarProductoCommand(int Id, string Nombre, string? Descripcion,
    decimal Precio, int Stock, int StockMinimo, int CategoriaId) : IRequest<ProductoDto>;

public sealed class ActualizarProductoHandler(IProductoRepository productos, ICategoriaRepository categorias)
    : IRequestHandler<ActualizarProductoCommand, ProductoDto>
{
    public async Task<ProductoDto> Handle(ActualizarProductoCommand request, CancellationToken ct)
    {
        var producto = await productos.ObtenerAsync(request.Id, ct)
            ?? throw new NoEncontradoException(Mensajes.ProductoNoEncontrado);

        if (!await categorias.ExisteActivaAsync(request.CategoriaId, ct))
            throw new DominioException(Mensajes.CategoriaInvalida);

        producto.Actualizar(request.Nombre, request.Descripcion, request.Precio,
            request.Stock, request.StockMinimo, request.CategoriaId);

        await productos.ActualizarAsync(producto, ct);
        return ProductoMapping.ToDto(producto);
    }
}
