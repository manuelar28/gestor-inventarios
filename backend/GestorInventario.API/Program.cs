using GestorInventario.API.Middleware;
using GestorInventario.Application.Abstractions;
using GestorInventario.Application.Productos.Commands;
using GestorInventario.Infrastructure.Persistence;
using GestorInventario.Infrastructure.Repositories;
using GestorInventario.Infrastructure.Seeds;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().ConfigureApiBehaviorOptions(options =>
    // Los errores de formato del cuerpo también responden con { "error": "mensaje" }.
    options.InvalidModelStateResponseFactory = context =>
    {
        var mensaje = context.ModelState.Values
            .SelectMany(v => v.Errors)
            .Select(e => e.ErrorMessage)
            .FirstOrDefault(m => !string.IsNullOrWhiteSpace(m)) ?? "La solicitud no es válida.";
        return new BadRequestObjectResult(new { error = mensaje });
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(CrearProductoCommand).Assembly));

builder.Services.AddDbContext<PersistenceContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Inventario")));
builder.Services.AddScoped<IProductoRepository, ProductoRepository>();
builder.Services.AddScoped<ICategoriaRepository, CategoriaRepository>();

var origenesPermitidos = builder.Configuration.GetSection("Cors:Origenes").Get<string[]>()
    ?? new[] { "http://localhost:4200" };
builder.Services.AddCors(options => options.AddPolicy("Frontend", policy =>
    policy.WithOrigins(origenesPermitidos).AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

await InicializarBaseDeDatosAsync(app);

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseSwagger();
app.UseSwaggerUI();
app.UseCors("Frontend");
app.MapControllers();

app.Run();

static async Task InicializarBaseDeDatosAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<PersistenceContext>();
        await DbSeeder.SembrarAsync(db);
        logger.LogInformation("Base de datos lista (esquema y datos iniciales verificados).");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "No fue posible inicializar la base de datos. Revisa la cadena de conexión 'Inventario' y que SQL Server esté activo.");
    }
}
