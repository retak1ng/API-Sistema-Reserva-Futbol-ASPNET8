using CourtBooking.Application.DTOs.Auth;
using CourtBooking.Domain.Primitives;
using MediatR;

namespace CourtBooking.Application.Features.Auth.Login;

public record LoginCommand(
    string Email,
    string Password
) : IRequest<Result<AuthResponse>>;