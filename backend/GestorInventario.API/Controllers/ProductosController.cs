using GestorInventario.API.Contracts;
using GestorInventario.Application.Productos;
using GestorInventario.Application.Productos.Commands;
using GestorInventario.Application.Productos.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GestorInventario.API.Controllers;

[ApiController]
[Route("api/productos")]
public sealed class ProductosController(ISender sender) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ProductoDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar(CancellationToken ct) =>
        Ok(await sender.Send(new ConsultarProductosQuery(), ct));

    [HttpGet("stock-bajo")]
    [ProducesResponseType(typeof(IReadOnlyList<ProductoDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> StockBajo(CancellationToken ct) =>
        Ok(await sender.Send(new ConsultarStockBajoQuery(), ct));

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ProductoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Obtener(int id, CancellationToken ct) =>
        Ok(await sender.Send(new ObtenerProductoQuery(id), ct));

    [HttpPost]
    [ProducesResponseType(typeof(ProductoDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Crear(CrearProductoRequest request, CancellationToken ct)
    {
        var creado = await sender.Send(new CrearProductoCommand(request.Nombre, request.Descripcion,
            request.Precio, request.Stock, request.StockMinimo, request.CategoriaId), ct);
        return CreatedAtAction(nameof(Obtener), new { id = creado.Id }, creado);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ProductoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Actualizar(int id, ActualizarProductoRequest request, CancellationToken ct) =>
        Ok(await sender.Send(new ActualizarProductoCommand(id, request.Nombre, request.Descripcion,
            request.Precio, request.Stock, request.StockMinimo, request.CategoriaId), ct));

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Eliminar(int id, CancellationToken ct)
    {
        await sender.Send(new EliminarProductoCommand(id), ct);
        return NoContent();
    }

    [HttpPatch("{id:int}/stock")]
    [ProducesResponseType(typeof(ProductoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AjustarStock(int id, AjustarStockRequest request, CancellationToken ct) =>
        Ok(await sender.Send(new AjustarStockCommand(id, request.Cantidad, request.Tipo), ct));
}
