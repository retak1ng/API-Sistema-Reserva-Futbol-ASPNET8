using CourtBooking.Domain.Enums;
using CourtBooking.Domain.Errors;
using CourtBooking.Domain.Primitives;

namespace CourtBooking.Domain.Entities;

public sealed class Reservation : Entity
{
    public string UserId { get; private set; } = string.Empty;
    public string CourtId { get; private set; } = string.Empty;
    public string AvailabilityId { get; private set; } = string.Empty;
    public DateOnly Date { get; private set; }
    public TimeOnly StartTime { get; private set; }
    public TimeOnly EndTime { get; private set; }
    public decimal TotalPrice { get; private set; }
    public ReservationStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Reservation(
        string id,
        string userId,
        string courtId,
        string availabilityId,
        DateOnly date,
        TimeOnly startTime,
        TimeOnly endTime,
        decimal totalPrice)
        : base(id)
    {
        UserId = userId;
        CourtId = courtId;
        AvailabilityId = availabilityId;
        Date = date;
        StartTime = startTime;
        EndTime = endTime;
        TotalPrice = totalPrice;
        Status = ReservationStatus.Confirmed;
        CreatedAt = DateTime.UtcNow;
    }

    private Reservation() { }

    public static Result<Reservation> Create(
        string userId,
        string courtId,
        string availabilityId,
        DateOnly date,
        TimeOnly startTime,
        TimeOnly endTime,
        decimal totalPrice)
    {
        if (totalPrice < 0)
            return Result.Failure<Reservation>(DomainErrors.Reservation.InvalidPrice);

        var reservation = new Reservation(
            id: Guid.NewGuid().ToString(),
            userId: userId,
            courtId: courtId,
            availabilityId: availabilityId,
            date: date,
            startTime: startTime,
            endTime: endTime,
            totalPrice: totalPrice
        );

        return Result.Success(reservation);
    }

    public Result Cancel()
    {
        if (Status == ReservationStatus.Cancelled)
            return Result.Failure(DomainErrors.Reservation.AlreadyCancelled);

        Status = ReservationStatus.Cancelled;
        return Result.Success();
    }
}