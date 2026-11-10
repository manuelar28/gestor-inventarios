using GestorInventario.Application.Abstractions;
using GestorInventario.Domain.Entities;
using GestorInventario.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GestorInventario.Infrastructure.Repositories;

public sealed class ProductoRepository(PersistenceContext db) : IProductoRepository
{
    public async Task<IReadOnlyList<Producto>> ListarAsync(CancellationToken ct) =>
        await db.Productos.AsNoTracking()
            .Include(p => p.Categoria)
            .OrderBy(p => p.Nombre)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<Producto>> ListarStockBajoAsync(CancellationToken ct) =>
        await db.Productos.AsNoTracking()
            .Include(p => p.Categoria)
            .Where(p => p.Stock < p.StockMinimo)
            .OrderBy(p => p.Nombre)
            .ToListAsync(ct);

    public Task<Producto?> ObtenerAsync(int id, CancellationToken ct) =>
        db.Productos.Include(p => p.Categoria).FirstOrDefaultAsync(p => p.Id == id, ct);

    public async Task<Producto> AgregarAsync(Producto producto, CancellationToken ct)
    {
        db.Productos.Add(producto);
        await db.SaveChangesAsync(ct);
        await db.Entry(producto).Reference(p => p.Categoria).LoadAsync(ct);
        return producto;
    }

    public async Task ActualizarAsync(Producto producto, CancellationToken ct)
    {
        await db.SaveChangesAsync(ct);
        // La categoría pudo cambiar: se recarga para devolver el nombre actualizado.
        var referencia = db.Entry(producto).Reference(p => p.Categoria);
        referencia.IsLoaded = false;
        await referencia.LoadAsync(ct);
    }

    public async Task EliminarAsync(Producto producto, CancellationToken ct)
    {
        db.Productos.Remove(producto);
        await db.SaveChangesAsync(ct);
    }
}
