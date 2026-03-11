namespace CourtBooking.Application.DTOs.Availability;

public record CreateAvailabilityRequest(
    string CourtId,
    DateOnly Date,
    TimeOnly StartTime,
    TimeOnly EndTime
);