namespace CourtBooking.Application.Abstractions.Services;

// Abstraer DateTime.UtcNow permite controlarlo en tests
public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
    DateOnly TodayUtc { get; }
}