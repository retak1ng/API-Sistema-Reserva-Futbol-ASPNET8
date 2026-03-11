using CourtBooking.Application.DTOs.Court;
using CourtBooking.Domain.Primitives;
using MediatR;

namespace CourtBooking.Application.Features.Courts.Create;

public record CreateCourtCommand(
    string Name,
    string Description,
    decimal PricePerHour
) : IRequest<Result<CourtResponse>>;