using GestorInventario.Domain.Entities;
using GestorInventario.Domain.Enums;
using GestorInventario.Domain.Exceptions;

namespace GestorInventario.Tests.Domain;

public class ProductoTests
{
    private static Producto Crear(int stock = 10, int stockMinimo = 5) =>
        Producto.Crear("Mouse", "Inalámbrico", 45000m, stock, stockMinimo, 1);

    [Fact]
    public void Crear_con_datos_validos_asigna_los_valores()
    {
        var producto = Producto.Crear("  Mouse  ", "  Inalámbrico  ", 45000m, 10, 5, 2);

        Assert.Equal("Mouse", producto.Nombre);
        Assert.Equal("Inalámbrico", producto.Descripcion);
        Assert.Equal(45000m, producto.Precio);
        Assert.Equal(10, producto.Stock);
        Assert.Equal(5, producto.StockMinimo);
        Assert.Equal(2, producto.CategoriaId);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Crear_con_precio_no_positivo_lanza_excepcion(decimal precio)
    {
        Assert.Throws<DominioException>(() => Producto.Crear("Mouse", null, precio, 1, 1, 1));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Crear_sin_nombre_lanza_excepcion(string nombre)
    {
        Assert.Throws<DominioException>(() => Producto.Crear(nombre, null, 100m, 1, 1, 1));
    }

    [Fact]
    public void Crear_con_stock_negativo_lanza_excepcion()
    {
        Assert.Throws<DominioException>(() => Producto.Crear("Mouse", null, 100m, -1, 1, 1));
    }

    [Fact]
    public void Crear_sin_categoria_lanza_excepcion()
    {
        Assert.Throws<DominioException>(() => Producto.Crear("Mouse", null, 100m, 1, 1, 0));
    }

    [Fact]
    public void Tiene_stock_bajo_cuando_el_stock_es_menor_al_minimo()
    {
        Assert.True(Crear(stock: 2, stockMinimo: 5).TieneStockBajo);
    }

    [Fact]
    public void No_tiene_stock_bajo_cuando_el_stock_es_igual_al_minimo()
    {
        Assert.False(Crear(stock: 5, stockMinimo: 5).TieneStockBajo);
    }

    [Fact]
    public void Entrada_suma_al_stock()
    {
        var producto = Crear(stock: 10);
        producto.AjustarStock(TipoMovimiento.Entrada, 5);
        Assert.Equal(15, producto.Stock);
    }

    [Fact]
    public void Salida_resta_del_stock()
    {
        var producto = Crear(stock: 10);
        producto.AjustarStock(TipoMovimiento.Salida, 4);
        Assert.Equal(6, producto.Stock);
    }

    [Fact]
    public void Salida_mayor_al_stock_lanza_excepcion_y_no_modifica_el_stock()
    {
        var producto = Crear(stock: 3);
        Assert.Throws<DominioException>(() => producto.AjustarStock(TipoMovimiento.Salida, 4));
        Assert.Equal(3, producto.Stock);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-2)]
    public void Ajustar_con_cantidad_no_positiva_lanza_excepcion(int cantidad)
    {
        var producto = Crear();
        Assert.Throws<DominioException>(() => producto.AjustarStock(TipoMovimiento.Entrada, cantidad));
    }
}
