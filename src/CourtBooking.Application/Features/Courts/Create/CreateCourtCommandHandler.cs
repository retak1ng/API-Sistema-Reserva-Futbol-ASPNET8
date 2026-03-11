using CourtBooking.Application.Abstractions.Repositories;
using CourtBooking.Application.DTOs.Court;
using CourtBooking.Domain.Entities;
using CourtBooking.Domain.Primitives;
using MediatR;

namespace CourtBooking.Application.Features.Courts.Create;

public class CreateCourtCommandHandler
    : IRequestHandler<CreateCourtCommand, Result<CourtResponse>>
{
    private readonly ICourtRepository _courtRepository;

    public CreateCourtCommandHandler(ICourtRepository courtRepository)
    {
        _courtRepository = courtRepository;
    }

    public async Task<Result<CourtResponse>> Handle(
        CreateCourtCommand request,
        CancellationToken cancellationToken)
    {
        var courtResult = Court.Create(
            name: request.Name,
            description: request.Description,
            pricePerHour: request.PricePerHour);

        if (courtResult.IsFailure)
            return Result.Failure<CourtResponse>(courtResult.Error);

        var court = courtResult.Value;

        await _courtRepository.AddAsync(court, cancellationToken);

        return Result.Success(new CourtResponse(
            Id: court.Id,
            Name: court.Name,
            Description: court.Description,
            PricePerHour: court.PricePerHour,
            IsActive: court.IsActive));
    }
}