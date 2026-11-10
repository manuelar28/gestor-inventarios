using GestorInventario.Application.Abstractions;
using GestorInventario.Application.Exceptions;
using MediatR;

namespace GestorInventario.Application.Productos.Queries;

public sealed record ConsultarProductosQuery : IRequest<IReadOnlyList<ProductoDto>>;
public sealed record ConsultarStockBajoQuery : IRequest<IReadOnlyList<ProductoDto>>;
public sealed record ObtenerProductoQuery(int Id) : IRequest<ProductoDto>;

public sealed class ConsultarProductosHandler(IProductoRepository productos)
    : IRequestHandler<ConsultarProductosQuery, IReadOnlyList<ProductoDto>>
{
    public async Task<IReadOnlyList<ProductoDto>> Handle(ConsultarProductosQuery request, CancellationToken ct)
        => (await productos.ListarAsync(ct)).Select(ProductoMapping.ToDto).ToList();
}

public sealed class ConsultarStockBajoHandler(IProductoRepository productos)
    : IRequestHandler<ConsultarStockBajoQuery, IReadOnlyList<ProductoDto>>
{
    public async Task<IReadOnlyList<ProductoDto>> Handle(ConsultarStockBajoQuery request, CancellationToken ct)
        => (await productos.ListarStockBajoAsync(ct)).Select(ProductoMapping.ToDto).ToList();
}

public sealed class ObtenerProductoHandler(IProductoRepository productos)
    : IRequestHandler<ObtenerProductoQuery, ProductoDto>
{
    public async Task<ProductoDto> Handle(ObtenerProductoQuery request, CancellationToken ct)
    {
        var producto = await productos.ObtenerAsync(request.Id, ct)
            ?? throw new NoEncontradoException(Mensajes.ProductoNoEncontrado);
        return ProductoMapping.ToDto(producto);
    }
}
