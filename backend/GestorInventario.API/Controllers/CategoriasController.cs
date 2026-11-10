using GestorInventario.Application.Categorias;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GestorInventario.API.Controllers;

[ApiController]
[Route("api/categorias")]
public sealed class CategoriasController(ISender sender) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<CategoriaDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar(CancellationToken ct) =>
        Ok(await sender.Send(new ConsultarCategoriasQuery(), ct));
}
