using Scalar.AspNetCore;

// Crear y configurar el builder de la aplicación (captura args de línea de comandos)
var builder = WebApplication.CreateBuilder(args);

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
