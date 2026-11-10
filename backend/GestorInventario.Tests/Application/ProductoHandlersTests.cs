using GestorInventario.Application.Abstractions;
using GestorInventario.Application.Exceptions;
using GestorInventario.Application.Productos.Commands;
using GestorInventario.Domain.Entities;
using GestorInventario.Domain.Exceptions;

namespace GestorInventario.Tests.Application;

public class ProductoHandlersTests
{
    [Fact]
    public async Task Crear_con_categoria_inexistente_lanza_excepcion_de_dominio()
    {
        var handler = new CrearProductoHandler(new ProductoRepositoryFalso(), new CategoriaRepositoryFalso(existe: false));
        var comando = new CrearProductoCommand("Mouse", null, 100m, 5, 2, 99);

        await Assert.ThrowsAsync<DominioException>(() => handler.Handle(comando, CancellationToken.None));
    }

    [Fact]
    public async Task Crear_con_datos_validos_guarda_el_producto_y_devuelve_el_dto()
    {
        var repositorio = new ProductoRepositoryFalso();
        var handler = new CrearProductoHandler(repositorio, new CategoriaRepositoryFalso(existe: true));

        var dto = await handler.Handle(new CrearProductoCommand("Mouse", "USB", 100m, 1, 5, 1), CancellationToken.None);

        Assert.Single(repositorio.Guardados);
        Assert.Equal("Mouse", dto.Nombre);
        Assert.True(dto.TieneStockBajo);
    }

    [Fact]
    public async Task Ajustar_stock_de_producto_inexistente_lanza_no_encontrado()
    {
        var handler = new AjustarStockHandler(new ProductoRepositoryFalso());

        await Assert.ThrowsAsync<NoEncontradoException>(() =>
            handler.Handle(new AjustarStockCommand(1, 2, "ENTRADA"), CancellationToken.None));
    }

    [Fact]
    public async Task Ajustar_stock_con_tipo_invalido_lanza_excepcion_de_dominio()
    {
        var repositorio = new ProductoRepositoryFalso();
        repositorio.Guardados.Add(Producto.Crear("Mouse", null, 100m, 5, 2, 1));
        var handler = new AjustarStockHandler(repositorio);

        await Assert.ThrowsAsync<DominioException>(() =>
            handler.Handle(new AjustarStockCommand(0, 2, "OTRO"), CancellationToken.None));
    }

    [Fact]
    public async Task Eliminar_producto_inexistente_lanza_no_encontrado()
    {
        var handler = new EliminarProductoHandler(new ProductoRepositoryFalso());

        await Assert.ThrowsAsync<NoEncontradoException>(() =>
            handler.Handle(new EliminarProductoCommand(10), CancellationToken.None));
    }

    private sealed class ProductoRepositoryFalso : IProductoRepository
    {
        public List<Producto> Guardados { get; } = [];

        public Task<IReadOnlyList<Producto>> ListarAsync(CancellationToken ct) =>
            Task.FromResult<IReadOnlyList<Producto>>(Guardados);

        public Task<IReadOnlyList<Producto>> ListarStockBajoAsync(CancellationToken ct) =>
            Task.FromResult<IReadOnlyList<Producto>>(Guardados.Where(p => p.TieneStockBajo).ToList());

        // El producto con Id 0 es el único que existe en el repositorio falso (Id sin asignar).
        public Task<Producto?> ObtenerAsync(int id, CancellationToken ct) =>
            Task.FromResult(Guardados.FirstOrDefault(p => p.Id == id));

        public Task<Producto> AgregarAsync(Producto producto, CancellationToken ct)
        {
            Guardados.Add(producto);
            return Task.FromResult(producto);
        }

        public Task ActualizarAsync(Producto producto, CancellationToken ct) => Task.CompletedTask;

        public Task EliminarAsync(Producto producto, CancellationToken ct)
        {
            Guardados.Remove(producto);
            return Task.CompletedTask;
        }
    }

    private sealed class CategoriaRepositoryFalso(bool existe) : ICategoriaRepository
    {
        public Task<IReadOnlyList<Categoria>> ListarActivasAsync(CancellationToken ct) =>
            Task.FromResult<IReadOnlyList<Categoria>>([]);

        public Task<bool> ExisteActivaAsync(int categoriaId, CancellationToken ct) => Task.FromResult(existe);
    }
}
