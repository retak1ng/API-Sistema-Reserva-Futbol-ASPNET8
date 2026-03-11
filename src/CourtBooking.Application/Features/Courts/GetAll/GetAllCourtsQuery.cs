using CourtBooking.Application.DTOs.Court;
using CourtBooking.Domain.Primitives;
using MediatR;

namespace CourtBooking.Application.Features.Courts.GetAll;

public record GetAllCourtsQuery() : IRequest<Result<List<CourtResponse>>>;