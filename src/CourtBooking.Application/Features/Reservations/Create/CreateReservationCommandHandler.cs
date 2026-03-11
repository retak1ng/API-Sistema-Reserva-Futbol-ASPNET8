using CourtBooking.Application.Abstractions.Repositories;
using CourtBooking.Application.DTOs.Reservation;
using CourtBooking.Domain.Entities;
using CourtBooking.Domain.Errors;
using CourtBooking.Domain.Primitives;
using MediatR;

namespace CourtBooking.Application.Features.Reservations.Create;

public class CreateReservationCommandHandler
    : IRequestHandler<CreateReservationCommand, Result<ReservationResponse>>
{
    private readonly IReservationRepository _reservationRepository;
    private readonly IAvailabilityRepository _availabilityRepository;
    private readonly ICourtRepository _courtRepository;

    public CreateReservationCommandHandler(
        IReservationRepository reservationRepository,
        IAvailabilityRepository availabilityRepository,
        ICourtRepository courtRepository)
    {
        _reservationRepository = reservationRepository;
        _availabilityRepository = availabilityRepository;
        _courtRepository = courtRepository;
    }

    public async Task<Result<ReservationResponse>> Handle(
        CreateReservationCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Buscar el slot de disponibilidad
        var availability = await _availabilityRepository.GetByIdAsync(
            request.AvailabilityId, cancellationToken);

        if (availability is null)
            return Result.Failure<ReservationResponse>(
                DomainErrors.Availability.NotFound);

        // 2. Buscar la cancha para calcular el precio
        var court = await _courtRepository.GetByIdAsync(
            availability.CourtId, cancellationToken);

        if (court is null)
            return Result.Failure<ReservationResponse>(DomainErrors.Court.NotFound);

        // 3. Intentar reservar el slot — acá está la protección contra doble reserva
        // Book() verifica IsBooked internamente y devuelve Failure si ya está ocupado
        var bookResult = availability.Book();

        if (bookResult.IsFailure)
            return Result.Failure<ReservationResponse>(bookResult.Error);

        // 4. Calcular precio total
        var hours = (decimal)(availability.EndTime - availability.StartTime).TotalHours;
        var totalPrice = court.PricePerHour * hours;

        // 5. Crear la reserva
        var reservationResult = Reservation.Create(
            userId: request.UserId,
            courtId: court.Id,
            availabilityId: availability.Id,
            date: availability.Date,
            startTime: availability.StartTime,
            endTime: availability.EndTime,
            totalPrice: totalPrice);

        if (reservationResult.IsFailure)
            return Result.Failure<ReservationResponse>(reservationResult.Error);

        var reservation = reservationResult.Value;

        // 6. Persistir ambos en orden: primero marcar disponibilidad, luego crear reserva
        await _availabilityRepository.UpdateAsync(availability, cancellationToken);
        await _reservationRepository.AddAsync(reservation, cancellationToken);

        return Result.Success(new ReservationResponse(
            Id: reservation.Id,
            UserId: reservation.UserId,
            CourtId: reservation.CourtId,
            AvailabilityId: reservation.AvailabilityId,
            Date: reservation.Date,
            StartTime: reservation.StartTime,
            EndTime: reservation.EndTime,
            TotalPrice: reservation.TotalPrice,
            Status: reservation.Status.ToString(),
            CreatedAt: reservation.CreatedAt));
    }
}