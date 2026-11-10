using GestorInventario.Application.Abstractions;
using GestorInventario.Domain.Entities;
using GestorInventario.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GestorInventario.Infrastructure.Repositories;

public sealed class CategoriaRepository(PersistenceContext db) : ICategoriaRepository
{
    public async Task<IReadOnlyList<Categoria>> ListarActivasAsync(CancellationToken ct) =>
        await db.Categorias.AsNoTracking()
            .Where(c => c.Activo)
            .OrderBy(c => c.Nombre)
            .ToListAsync(ct);

    public Task<bool> ExisteActivaAsync(int categoriaId, CancellationToken ct) =>
        db.Categorias.AnyAsync(c => c.Id == categoriaId && c.Activo, ct);
}
