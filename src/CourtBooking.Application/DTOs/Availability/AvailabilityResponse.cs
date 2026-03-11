namespace CourtBooking.Application.DTOs.Availability;

public record AvailabilityResponse(
    string Id,
    string CourtId,
    DateOnly Date,
    TimeOnly StartTime,
    TimeOnly EndTime,
    bool IsBooked
);