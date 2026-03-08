using CourtBooking.Domain.Errors;
using CourtBooking.Domain.Primitives;

namespace CourtBooking.Domain.Entities;

public sealed class Availability : Entity
{
    public string CourtId { get; private set; } = string.Empty;
    public DateOnly Date { get; private set; }
    public TimeOnly StartTime { get; private set; }
    public TimeOnly EndTime { get; private set; }
    public bool IsBooked { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Availability(
        string id,
        string courtId,
        DateOnly date,
        TimeOnly startTime,
        TimeOnly endTime)
        : base(id)
    {
        CourtId = courtId;
        Date = date;
        StartTime = startTime;
        EndTime = endTime;
        IsBooked = false;
        CreatedAt = DateTime.UtcNow;
    }

    private Availability() { }

    public static Result<Availability> Create(
        string courtId,
        DateOnly date,
        TimeOnly startTime,
        TimeOnly endTime)
    {
        if (string.IsNullOrWhiteSpace(courtId))
            return Result.Failure<Availability>(DomainErrors.Availability.EmptyCourtId);

        if (startTime >= endTime)
            return Result.Failure<Availability>(DomainErrors.Availability.InvalidTimeRange);

        if (date < DateOnly.FromDateTime(DateTime.UtcNow))
            return Result.Failure<Availability>(DomainErrors.Availability.PastDate);

        var availability = new Availability(
            id: Guid.NewGuid().ToString(),
            courtId: courtId,
            date: date,
            startTime: startTime,
            endTime: endTime
        );

        return Result.Success(availability);
    }

    // ← Esta es la regla de negocio más importante del sistema
    public Result Book()
    {
        if (IsBooked)
            return Result.Failure(DomainErrors.Availability.AlreadyBooked);

        IsBooked = true;
        return Result.Success();
    }

    public Result Release()
    {
        if (!IsBooked)
            return Result.Failure(DomainErrors.Availability.NotBooked);

        IsBooked = false;
        return Result.Success();
    }
}