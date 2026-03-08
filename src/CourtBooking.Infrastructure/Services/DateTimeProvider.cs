using CourtBooking.Application.Abstractions.Services;

namespace CourtBooking.Infrastructure.Services;

// Implementación concreta — centraliza el acceso a la fecha/hora
// En tests podemos mockear IDateTimeProvider para controlar el tiempo
internal sealed class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
    public DateOnly TodayUtc => DateOnly.FromDateTime(DateTime.UtcNow);
}