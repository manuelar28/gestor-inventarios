using GestorInventario.Application.Abstractions;
using GestorInventario.Domain.Entities;
using GestorInventario.Domain.Exceptions;
using MediatR;

namespace GestorInventario.Application.Productos.Commands;

public sealed record CrearProductoCommand(string Nombre, string? Descripcion, decimal Precio,
    int Stock, int StockMinimo, int CategoriaId) : IRequest<ProductoDto>;

public sealed class CrearProductoHandler(IProductoRepository productos, ICategoriaRepository categorias)
    : IRequestHandler<CrearProductoCommand, ProductoDto>
{
    public async Task<ProductoDto> Handle(CrearProductoCommand request, CancellationToken ct)
    {
        if (!await categorias.ExisteActivaAsync(request.CategoriaId, ct))
            throw new DominioException(Mensajes.CategoriaInvalida);

        var producto = Producto.Crear(request.Nombre, request.Descripcion, request.Precio,
            request.Stock, request.StockMinimo, request.CategoriaId);

        var creado = await productos.AgregarAsync(producto, ct);
        return ProductoMapping.ToDto(creado);
    }
}
