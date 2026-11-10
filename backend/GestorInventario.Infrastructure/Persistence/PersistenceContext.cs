using GestorInventario.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GestorInventario.Infrastructure.Persistence;

public sealed class PersistenceContext(DbContextOptions<PersistenceContext> options) : DbContext(options)
{
    public DbSet<Producto> Productos => Set<Producto>();
    public DbSet<Categoria> Categorias => Set<Categoria>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Categoria>(e =>
        {
            e.ToTable("Categorias");
            e.HasKey(x => x.Id).HasName("PK_Categorias");
            e.Property(x => x.Nombre).HasMaxLength(Categoria.LongitudMaximaNombre).IsRequired();
            e.Property(x => x.Activo).IsRequired();
        });

        modelBuilder.Entity<Producto>(e =>
        {
            e.ToTable("Productos", t =>
            {
                t.HasCheckConstraint("CK_Productos_Stock", "[Stock] >= 0");
                t.HasCheckConstraint("CK_Productos_Precio", "[Precio] > 0");
                t.HasCheckConstraint("CK_Productos_StockMinimo", "[StockMinimo] >= 0");
            });
            e.HasKey(x => x.Id).HasName("PK_Productos");
            e.Property(x => x.Nombre).HasMaxLength(Producto.LongitudMaximaNombre).IsRequired();
            e.Property(x => x.Descripcion).HasMaxLength(Producto.LongitudMaximaDescripcion);
            e.Property(x => x.Precio).HasPrecision(10, 2);
            e.Property(x => x.FechaCreacion).HasDefaultValueSql("GETDATE()");
            e.Ignore(x => x.TieneStockBajo);
            e.HasOne(x => x.Categoria)
                .WithMany()
                .HasForeignKey(x => x.CategoriaId)
                .HasConstraintName("FK_Productos_Categorias")
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
