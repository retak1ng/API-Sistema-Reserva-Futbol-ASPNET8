using CourtBooking.API.Middleware;
using CourtBooking.API.Extensions;
using CourtBooking.Application;
using CourtBooking.Infrastructure;
using CourtBooking.Infrastructure.Logging;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// ── Logging con Serilog ──
builder.AddSerilogLogging();

try
{
    Log.Information("Starting Court Booking System API");

    // ── Capas de la aplicación ──
    builder.Services.AddApplication();
    builder.Services.AddInfrastructure(builder.Configuration);

    // ── API ──
    builder.Services.AddControllers()
        .AddJsonOptions(options =>
        {
            // Serializar enums como strings en lugar de números
            options.JsonSerializerOptions.Converters.Add(
                new System.Text.Json.Serialization.JsonStringEnumConverter());
        });

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerWithAuth();

    // ── CORS para desarrollo ──
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAll", policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
    });

    var app = builder.Build();

    // ── Middleware Pipeline (orden importa) ──
    app.UseMiddleware<GlobalExceptionMiddleware>();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Court Booking API v1");
            options.RoutePrefix = string.Empty; // Swagger en la raíz
        });
    }

    app.UseSerilogRequestLogging();
    app.UseCors("AllowAll");
    app.UseHttpsRedirection();
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

// Necesario para los integration tests
public partial class Program { }