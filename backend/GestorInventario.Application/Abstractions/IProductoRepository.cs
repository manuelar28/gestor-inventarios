using GestorInventario.Domain.Entities;

namespace GestorInventario.Application.Abstractions;

public interface IProductoRepository
{
    Task<IReadOnlyList<Producto>> ListarAsync(CancellationToken ct);
    Task<IReadOnlyList<Producto>> ListarStockBajoAsync(CancellationToken ct);
    Task<Producto?> ObtenerAsync(int id, CancellationToken ct);
    Task<Producto> AgregarAsync(Producto producto, CancellationToken ct);
    Task ActualizarAsync(Producto producto, CancellationToken ct);
    Task EliminarAsync(Producto producto, CancellationToken ct);
}
