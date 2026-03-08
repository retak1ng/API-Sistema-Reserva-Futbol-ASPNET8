using CourtBooking.Application.Abstractions.Repositories;
using CourtBooking.Application.Abstractions.Services;
using CourtBooking.Infrastructure.Authentication;
using CourtBooking.Infrastructure.Persistence.Firebase;
using CourtBooking.Infrastructure.Persistence.Firebase.Repositories;
using CourtBooking.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace CourtBooking.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // ── Settings fuertemente tipados ──
        services.Configure<JwtSettings>(
            configuration.GetSection(JwtSettings.SectionName));

        services.Configure<FirebaseSettings>(
            configuration.GetSection(FirebaseSettings.SectionName));

        // ── Firebase ──
        services.AddSingleton<FirebaseContext>();

        // ── Repositorios ──
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ICourtRepository, CourtRepository>();
        services.AddScoped<IAvailabilityRepository, AvailabilityRepository>();
        services.AddScoped<IReservationRepository, ReservationRepository>();

        // ── Servicios ──
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

        // ── JWT Authentication ──
        var jwtSettings = configuration
            .GetSection(JwtSettings.SectionName)
            .Get<JwtSettings>()!;

        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtSettings.Secret))
                };
            });

        services.AddAuthorization();

        return services;
    }
}