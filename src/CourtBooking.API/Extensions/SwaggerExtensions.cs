using Microsoft.OpenApi.Models;

namespace CourtBooking.API.Extensions;

public static class SwaggerExtensions
{
    public static IServiceCollection AddSwaggerWithAuth(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Court Booking System API",
                Version = "v1",
                Description = "API para gestión de reservas de canchas deportivas.",
                Contact = new OpenApiContact
                {
                    Name = "Tu Nombre",
                    Email = "tu@email.com"
                }
            });

            // Configurar autenticación JWT en Swagger
            var securityScheme = new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Description = "Ingresá el token JWT así: Bearer {token}",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT"
            };

            options.AddSecurityDefinition("Bearer", securityScheme);

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });

            // Incluir comentarios XML en Swagger
            var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
                options.IncludeXmlComments(xmlPath);
        });

        return services;
    }
}