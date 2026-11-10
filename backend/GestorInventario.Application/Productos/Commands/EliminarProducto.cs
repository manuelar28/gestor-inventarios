using GestorInventario.Application.Abstractions;
using GestorInventario.Application.Exceptions;
using MediatR;

namespace GestorInventario.Application.Productos.Commands;

public sealed record EliminarProductoCommand(int Id) : IRequest;

public sealed class EliminarProductoHandler(IProductoRepository productos)
    : IRequestHandler<EliminarProductoCommand>
{
    public async Task Handle(EliminarProductoCommand request, CancellationToken ct)
    {
        var producto = await productos.ObtenerAsync(request.Id, ct)
            ?? throw new NoEncontradoException(Mensajes.ProductoNoEncontrado);

        await productos.EliminarAsync(producto, ct);
    }
}
