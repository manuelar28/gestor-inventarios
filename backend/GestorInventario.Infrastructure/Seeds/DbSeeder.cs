using GestorInventario.Domain.Entities;
using GestorInventario.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GestorInventario.Infrastructure.Seeds;

/// <summary>
/// Crea el esquema si la base no existe y carga datos iniciales (categorías y productos de ejemplo)
/// solo cuando las tablas están vacías. Es seguro ejecutarlo varias veces.
/// </summary>
public static class DbSeeder
{
    public static async Task SembrarAsync(PersistenceContext db, CancellationToken ct = default)
    {
        await db.Database.EnsureCreatedAsync(ct);

        if (!await db.Categorias.AnyAsync(ct))
        {
            db.Categorias.AddRange(
                Categoria.Crear("Electrónica"),
                Categoria.Crear("Oficina"),
                Categoria.Crear("Aseo"));
            await db.SaveChangesAsync(ct);
        }

        if (await db.Productos.AnyAsync(ct)) return;

        var categorias = await db.Categorias.AsNoTracking().OrderBy(c => c.Id).ToListAsync(ct);
        if (categorias.Count == 0) return;

        int IdDe(string nombre) =>
            (categorias.FirstOrDefault(c => c.Nombre == nombre) ?? categorias[0]).Id;

        db.Productos.AddRange(
            Producto.Crear("Mouse inalámbrico", "Mouse óptico USB", 45000m, 12, 5, IdDe("Electrónica")),
            Producto.Crear("Teclado mecánico", "Teclado para oficina", 160000m, 2, 5, IdDe("Electrónica")),
            Producto.Crear("Cuaderno", "Cuaderno de 100 hojas", 8500m, 30, 10, IdDe("Oficina")),
            Producto.Crear("Resma de papel carta", "500 hojas", 22000m, 3, 10, IdDe("Oficina")),
            Producto.Crear("Detergente", "Detergente multiusos", 12000m, 3, 8, IdDe("Aseo")),
            Producto.Crear("Jabón líquido", "Jabón líquido 1 L", 9500m, 25, 8, IdDe("Aseo")));

        await db.SaveChangesAsync(ct);
    }
}
