using GestorInventario.Domain.Entities;

namespace GestorInventario.Application.Abstractions;

public interface ICategoriaRepository
{
    Task<IReadOnlyList<Categoria>> ListarActivasAsync(CancellationToken ct);
    Task<bool> ExisteActivaAsync(int categoriaId, CancellationToken ct);
}
