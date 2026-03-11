using CourtBooking.Application.Abstractions.Repositories;
using CourtBooking.Application.DTOs.Court;
using CourtBooking.Domain.Primitives;
using MediatR;

namespace CourtBooking.Application.Features.Courts.GetAll;

public class GetAllCourtsQueryHandler
    : IRequestHandler<GetAllCourtsQuery, Result<List<CourtResponse>>>
{
    private readonly ICourtRepository _courtRepository;

    public GetAllCourtsQueryHandler(ICourtRepository courtRepository)
    {
        _courtRepository = courtRepository;
    }

    public async Task<Result<List<CourtResponse>>> Handle(
        GetAllCourtsQuery request,
        CancellationToken cancellationToken)
    {
        var courts = await _courtRepository.GetAllActiveAsync(cancellationToken);

        var response = courts.Select(c => new CourtResponse(
            Id: c.Id,
            Name: c.Name,
            Description: c.Description,
            PricePerHour: c.PricePerHour,
            IsActive: c.IsActive))
            .ToList();

        return Result.Success(response);
    }
}