using CourtBooking.Application.Features.Auth.Login;
using CourtBooking.Application.Abstractions.Repositories;
using CourtBooking.Application.Tests.Helpers;
using CourtBooking.Domain.Errors;
using FluentAssertions;
using Moq;

namespace CourtBooking.Application.Tests.Features.Auth;

public class LoginCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly LoginCommandHandler _handler;
    private const string ValidPassword = "Password1";

    public LoginCommandHandlerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        var jwtGeneratorMock = MockHelpers.CreateJwtGeneratorMock();

        _handler = new LoginCommandHandler(
            _userRepositoryMock.Object,
            jwtGeneratorMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidCredentials_ReturnsSuccessWithToken()
    {
        // Arrange
        var user = MockHelpers.CreateValidUser();
        var command = new LoginCommand(Email: user.Email, Password: ValidPassword);

        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Token.Should().NotBeNullOrEmpty();
        result.Value.Email.Should().Be(user.Email);
    }

    [Fact]
    public async Task Handle_WithNonExistentEmail_ReturnsInvalidCredentialsError()
    {
        // Arrange
        var command = new LoginCommand(
            Email: "noexiste@email.com",
            Password: ValidPassword);

        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.User?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();

        // Importante: mismo error que contraseña incorrecta — no revelar cuál falló
        result.Error.Should().Be(DomainErrors.User.InvalidCredentials);
    }

    [Fact]
    public async Task Handle_WithWrongPassword_ReturnsInvalidCredentialsError()
    {
        // Arrange
        var user = MockHelpers.CreateValidUser();
        var command = new LoginCommand(Email: user.Email, Password: "WrongPassword1");

        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.User.InvalidCredentials);
    }
}