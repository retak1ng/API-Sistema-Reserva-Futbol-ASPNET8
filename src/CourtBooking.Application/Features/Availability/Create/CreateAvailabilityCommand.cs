using CourtBooking.Application.DTOs.Availability;
using CourtBooking.Domain.Primitives;
using MediatR;

namespace CourtBooking.Application.Features.Availability.Create;

public record CreateAvailabilityCommand(
    string CourtId,
    DateOnly Date,
    TimeOnly StartTime,
    TimeOnly EndTime
) : IRequest<Result<AvailabilityResponse>>;