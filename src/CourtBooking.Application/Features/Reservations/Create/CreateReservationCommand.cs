using CourtBooking.Application.DTOs.Reservation;
using CourtBooking.Domain.Primitives;
using MediatR;

namespace CourtBooking.Application.Features.Reservations.Create;

public record CreateReservationCommand(
    string UserId,
    string AvailabilityId
) : IRequest<Result<ReservationResponse>>;