using CourtBooking.Application.Behaviors;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace CourtBooking.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Registra todos los Handlers automáticamente
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

        // Registra todos los Validators automáticamente
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        // Pipeline: Logging → Validation → Handler
        services.AddTransient(
            typeof(IPipelineBehavior<,>),
            typeof(LoggingBehavior<,>));

        services.AddTransient(
            typeof(IPipelineBehavior<,>),
            typeof(ValidationBehavior<,>));

        return services;
    }
}