using CourtBooking.Domain.Entities;
using CourtBooking.Domain.Enums;
using CourtBooking.Domain.Errors;
using FluentAssertions;

namespace CourtBooking.Domain.Tests.Entities;

public class UserTests
{
    #region Create

    [Fact]
    public void Create_WithValidData_ReturnsSuccessWithDefaultUserRole()
    {
        // Act
        var result = User.Create(
            name: "Martin Rojas",
            email: "martin@email.com",
            passwordHash: "hashedPassword123");

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Role.Should().Be(UserRole.User);
        result.Value.Email.Should().Be("martin@email.com");
    }

    [Fact]
    public void Create_WithAdminRole_ReturnsSuccessWithAdminRole()
    {
        // Act
        var result = User.Create(
            name: "Admin",
            email: "admin@email.com",
            passwordHash: "hashedPassword123",
            role: UserRole.Admin);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Role.Should().Be(UserRole.Admin);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Create_WithEmptyName_ReturnsFailureResult(string? name)
    {
        // Act
        var result = User.Create(
            name: name!,
            email: "test@email.com",
            passwordHash: "hashedPassword123");

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.User.EmptyName);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Create_WithEmptyEmail_ReturnsFailureResult(string? email)
    {
        // Act
        var result = User.Create(
            name: "Martin",
            email: email!,
            passwordHash: "hashedPassword123");

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.User.EmptyEmail);
    }

    [Fact]
    public void Create_EmailShouldBeNormalizedToLowercase()
    {
        // Act
        var result = User.Create(
            name: "Martin",
            email: "MARTIN@EMAIL.COM",
            passwordHash: "hashedPassword123");

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Email.Should().Be("martin@email.com");
    }

    #endregion
}