using Microsoft.AspNetCore.Builder;
using Serilog;
using Serilog.Events;

namespace CourtBooking.Infrastructure.Logging;

public static class SerilogConfiguration
{
    public static WebApplicationBuilder AddSerilogLogging(this WebApplicationBuilder builder)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("System", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .Enrich.WithEnvironmentName()
            .WriteTo.Console(outputTemplate:
                "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} " +
                "{Properties:j}{NewLine}{Exception}")
            .WriteTo.File(
                path: "logs/courtbooking-.log",
                rollingInterval: RollingInterval.Day,
                outputTemplate:
                    "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] " +
                    "{Message:lj}{NewLine}{Exception}")
            .CreateLogger();

        builder.Host.UseSerilog();

        return builder;
    }
}