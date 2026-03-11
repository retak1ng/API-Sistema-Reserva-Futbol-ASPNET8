namespace CourtBooking.Application.DTOs.Reservation;

public record ReservationResponse(
    string Id,
    string UserId,
    string CourtId,
    string AvailabilityId,
    DateOnly Date,
    TimeOnly StartTime,
    TimeOnly EndTime,
    decimal TotalPrice,
    string Status,
    DateTime CreatedAt
);