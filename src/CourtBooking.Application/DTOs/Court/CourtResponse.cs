namespace CourtBooking.Application.DTOs.Court;

public record CourtResponse(
    string Id,
    string Name,
    string Description,
    decimal PricePerHour,
    bool IsActive
);