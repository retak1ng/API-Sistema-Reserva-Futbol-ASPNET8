using CourtBooking.Application.Abstractions.Repositories;
using CourtBooking.Application.Abstractions.Services;
using CourtBooking.Domain.Entities;
using CourtBooking.Domain.Enums;
using Moq;

namespace CourtBooking.Application.Tests.Helpers;

// Helpers para crear mocks pre-configurados
public static class MockHelpers
{
    public static User CreateValidUser(UserRole role = UserRole.User)
    {
        return User.Create(
            name: "Test User",
            email: "test@email.com",
            passwordHash: BCrypt.Net.BCrypt.HashPassword("Password1"),
            role: role).Value;
    }

    public static Court CreateValidCourt(decimal pricePerHour = 100m)
    {
        return Court.Create(
            name: "Cancha 1",
            description: "Cancha de fútbol",
            pricePerHour: pricePerHour).Value;
    }

    public static Availability CreateValidAvailability(string? courtId = null)
    {
        return Availability.Create(
            courtId: courtId ?? Guid.NewGuid().ToString(),
            date: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
            startTime: new TimeOnly(10, 0),
            endTime: new TimeOnly(11, 0)).Value;
    }

    public static Mock<IJwtTokenGenerator> CreateJwtGeneratorMock(string token = "fake.jwt.token")
    {
        var mock = new Mock<IJwtTokenGenerator>();
        mock.Setup(x => x.GenerateToken(It.IsAny<User>())).Returns(token);
        return mock;
    }
}