using GestorInventario.Application.Abstractions;
using MediatR;

namespace GestorInventario.Application.Categorias;

public sealed record CategoriaDto(int Id, string Nombre);

public sealed record ConsultarCategoriasQuery : IRequest<IReadOnlyList<CategoriaDto>>;

public sealed class ConsultarCategoriasHandler(ICategoriaRepository categorias)
    : IRequestHandler<ConsultarCategoriasQuery, IReadOnlyList<CategoriaDto>>
{
    public async Task<IReadOnlyList<CategoriaDto>> Handle(ConsultarCategoriasQuery request, CancellationToken ct)
        => (await categorias.ListarActivasAsync(ct))
            .Select(c => new CategoriaDto(c.Id, c.Nombre))
            .ToList();
}
