using Ecommerce_Api.Data;
using Ecommerce_Api.Repository;
using Ecommerce_Api.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

// Crear y configurar el builder de la aplicación (captura args de línea de comandos)
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConectionSql")));

builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
// Registra AutoMapper en el contenedor de inyección de dependencias
// AddAutoMapper = agrega AutoMapper como servicio
// cfg.AddMaps(...) = escanea el ensamblado actual en busca de perfiles de m
builder.Services.AddAutoMapper(cfg => 
{
    cfg.AddMaps(typeof(Program).Assembly);
});

// Añadir servicios al contenedor de inyección de dependencias
builder.Services.AddControllers();            // Soporte para controladores API (MVC)
// Servicios para OpenAPI/Swagger (documentación y UI)
builder.Services.AddOpenApi();

// Construir la aplicación a partir del builder configurado
var app = builder.Build();

// Configuración del pipeline HTTP (solo activo en entorno Development)
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();             // Mapea endpoints y UI de OpenAPI/Swagger
    app.MapScalarApiReference();  // Registra referencias/recursos adicionales de Scalar
}

app.UseHttpsRedirection(); // Redirige solicitudes HTTP a HTTPS

app.UseAuthorization();    // Habilita middleware de autorización (políticas/atributos)

app.MapControllers();      // Mapea rutas a los controladores registrados en los servicios

app.Run();                 // Inicia la aplicación y comienza a escuchar peticiones
