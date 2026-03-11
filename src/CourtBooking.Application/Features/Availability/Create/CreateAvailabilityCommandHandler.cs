using CourtBooking.Application.Abstractions.Repositories;
using CourtBooking.Application.DTOs.Availability;
using AvailabilityEntity = CourtBooking.Domain.Entities.Availability;
using CourtBooking.Domain.Errors;
using CourtBooking.Domain.Primitives;
using MediatR;

namespace CourtBooking.Application.Features.Availability.Create;

public class CreateAvailabilityCommandHandler
    : IRequestHandler<CreateAvailabilityCommand, Result<AvailabilityResponse>>
{
    private readonly IAvailabilityRepository _availabilityRepository;
    private readonly ICourtRepository _courtRepository;

    public CreateAvailabilityCommandHandler(
        IAvailabilityRepository availabilityRepository,
        ICourtRepository courtRepository)
    {
        _availabilityRepository = availabilityRepository;
        _courtRepository = courtRepository;
    }

    public async Task<Result<AvailabilityResponse>> Handle(
        CreateAvailabilityCommand request,
        CancellationToken cancellationToken)
    {
        // Verificar que la cancha existe
        var court = await _courtRepository.GetByIdAsync(
            request.CourtId, cancellationToken);

        if (court is null)
            return Result.Failure<AvailabilityResponse>(DomainErrors.Court.NotFound);

        var availabilityResult = AvailabilityEntity.Create(
            courtId: request.CourtId,
            date: request.Date,
            startTime: request.StartTime,
            endTime: request.EndTime);

        if (availabilityResult.IsFailure)
            return Result.Failure<AvailabilityResponse>(availabilityResult.Error);

        var availability = availabilityResult.Value;

        await _availabilityRepository.AddAsync(availability, cancellationToken);

        return Result.Success(new AvailabilityResponse(
            Id: availability.Id,
            CourtId: availability.CourtId,
            Date: availability.Date,
            StartTime: availability.StartTime,
            EndTime: availability.EndTime,
            IsBooked: availability.IsBooked));
    }
}