using CourtBooking.Application.DTOs.Auth;
using CourtBooking.Domain.Primitives;
using MediatR;

namespace CourtBooking.Application.Features.Auth.Register;

// IRequest<T> → este Command devuelve un Result<AuthResponse>
public record RegisterCommand(
    string Name,
    string Email,
    string Password
) : IRequest<Result<AuthResponse>>;