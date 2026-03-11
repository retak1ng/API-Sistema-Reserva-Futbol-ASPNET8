namespace CourtBooking.Application.DTOs.Court;

public record CreateCourtRequest(
    string Name,
    string Description,
    decimal PricePerHour
);