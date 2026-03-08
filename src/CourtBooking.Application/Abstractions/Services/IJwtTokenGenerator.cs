using CourtBooking.Domain.Entities;

namespace CourtBooking.Application.Abstractions.Services;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user);
}